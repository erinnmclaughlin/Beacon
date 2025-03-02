using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;

namespace Beacon.API.IntegrationTests;

public abstract class IntegrationTestBase(TestFixture fixture) : IAsyncLifetime, IClassFixture<TestFixture>
{
    private Respawner? _respawnCheckpoint;
    
    /// <summary>
    /// The cancellation token for the current test context.
    /// </summary>
    protected static CancellationToken AbortTest => TestContext.Current.CancellationToken;
    
    /// <summary>
    /// A reference to the <see cref="BeaconDbContext"/> in the current test scope.
    /// </summary>
    protected BeaconDbContext DbContext => fixture.DbContext;

    /// <summary>
    /// An <see cref="HttpClient"/> instance configured to call the web API.
    /// </summary>
    protected HttpClient HttpClient { get; } = fixture.CreateClient();
    
    /// <inheritdoc />
    public virtual async ValueTask InitializeAsync()
    {
        if (!fixture.IsSeeded)
        {
            await fixture.ApplySeedData(GetAllSeedData());
        }

        if (_respawnCheckpoint is null)
        {
            await CreateCheckpoint(GetAllSeedData()
                .Select(t => DbContext.Model.FindEntityType(t.GetType())!.GetTableName()!)
                .Select(t => new Table(t))
                .Distinct()
                .ToArray()
            );
        }
    }

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        
        await SendAsync(new LogoutRequest());
        HttpClient.Dispose();
    }

    /// <summary>
    /// Enumerates the default seed data common to all tests.
    /// </summary>
    protected virtual IEnumerable<object> EnumerateInitialSeedData()
    {
        yield return TestData.Lab;
        
        yield return TestData.AdminUser;
        yield return new Membership
        {
            LaboratoryId = TestData.Lab.Id,
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Admin
        };
        
        yield return TestData.ManagerUser;
        yield return new Membership
        {
            LaboratoryId = TestData.Lab.Id,
            MemberId = TestData.ManagerUser.Id,
            MembershipType = LaboratoryMembershipType.Manager
        };
        
        yield return TestData.AnalystUser;
        yield return new Membership
        {
            LaboratoryId = TestData.Lab.Id,
            MemberId = TestData.AnalystUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };
        
        yield return TestData.MemberUser;
        yield return new Membership
        {
            LaboratoryId = TestData.Lab.Id,
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Member
        };
        
        yield return TestData.NonMemberUser;
    }
    
    /// <summary>
    /// Enumerates the additional seed data for a specific class of tests.
    /// </summary>
    protected virtual IEnumerable<object> EnumerateReseedData()
    {
        yield break;
    }

    /// <summary>
    /// Adds the specified <paramref name="data"/> to the database.
    /// </summary>
    /// <param name="data">The data to add.</param>
    protected async Task AddDataAsync(params object[] data)
    {
        DbContext.AddRange(data);
        await DbContext.SaveChangesAsync(AbortTest);
    }
    
    /// <summary>
    /// Gets the default user for a given <paramref name="membershipType"/>.
    /// </summary>
    /// <param name="membershipType">The membership type to get the default user for.</param>
    protected User GetDefaultUserForMembershipType(LaboratoryMembershipType membershipType) => membershipType switch
    {
        LaboratoryMembershipType.Admin => TestData.AdminUser,
        LaboratoryMembershipType.Manager => TestData.ManagerUser,
        LaboratoryMembershipType.Analyst => TestData.AnalystUser,
        LaboratoryMembershipType.Member => TestData.MemberUser,
        _ => throw new ArgumentOutOfRangeException(nameof(membershipType), membershipType, null)
    };

    /// <summary>
    /// Log in as a specific user.
    /// </summary>
    /// <param name="user">The user to log in as.</param>
    protected Task<HttpResponseMessage> LoginAs(User user) => SendAsync(new LoginRequest
    {
        EmailAddress = user.EmailAddress,
        Password = $"!!{user.DisplayName.ToLower()}"
    });

    /// <summary>
    /// Log in as a specified user and set the current lab to <see cref="TestData.Lab"/>.
    /// </summary>
    /// <param name="user">The user to log in as.</param>
    protected async Task LogInToDefaultLab(User user)
    {
        await LogInToLab(user, TestData.Lab.Id);
    }

    /// <summary>
    /// Log in as a specified user and set the current lab to <paramref name="currentLabId"/>.
    /// </summary>
    /// <param name="user">The user to log in as.</param>
    /// <param name="currentLabId">The ID of the lab to set as the current lab.</param>
    protected async Task LogInToLab(User user, Guid? currentLabId)
    {
        var loginResponse = await LoginAs(user);
        loginResponse.EnsureSuccessStatusCode();
        
        var setCurrentLabResponse = await SetCurrentLab(currentLabId);
        setCurrentLabResponse.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Sets the laboratory that requests will be scoped to.
    /// </summary>
    /// <param name="labId">The ID of the lab, or null.</param>
    protected Task<HttpResponseMessage> SetCurrentLab(Guid? labId) => SendAsync(new SetCurrentLaboratoryRequest
    {
        LaboratoryId = labId
    });
    
    protected Task<HttpResponseMessage> SendAsync<TRequest>(BeaconRequest<TRequest> request)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new()
    {
        return HttpClient.SendAsync(request, AbortTest);
    }
    
    protected Task<HttpResponseMessage> SendAsync<TRequest, TResponse>(BeaconRequest<TRequest, TResponse> request)
        where TRequest : BeaconRequest<TRequest, TResponse>, IBeaconRequest<TRequest>, new()
    {
        return HttpClient.SendAsync(request, AbortTest);
    }

    private async Task CreateCheckpoint(params Table[] tablesToInclude)
    {
        var options = new RespawnerOptions
        {
            TablesToIgnore = ["__EFMigrationsHistory"],
            TablesToInclude = tablesToInclude
        };

        _respawnCheckpoint = await Respawner.CreateAsync(fixture.ConnectionString, options);
    }

    protected async Task ResetDatabase()
    {
        if (_respawnCheckpoint is null)
            throw new InvalidOperationException("The respawn checkpoint has not been set.");
        
        await _respawnCheckpoint.ResetAsync(fixture.ConnectionString);
        await fixture.ApplySeedData(GetAllSeedData());
    }

    private object[] GetAllSeedData() => EnumerateInitialSeedData().Concat(EnumerateReseedData()).Distinct().ToArray();
}
