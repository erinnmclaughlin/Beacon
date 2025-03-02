using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests;

[Collection(nameof(ContainerFixtureCollection))]
public abstract class IntegrationTestBase(TestFixture fixture) : IAsyncLifetime, IClassFixture<TestFixture>
{
    /// <summary>
    /// A reference to the <see cref="BeaconDbContext"/> in the current test scope.
    /// </summary>
    protected BeaconDbContext DbContext => fixture.Container.DbContext;

    /// <summary>
    /// An <see cref="HttpClient"/> instance configured to call the web API.
    /// </summary>
    protected HttpClient HttpClient { get; } = fixture.CreateClient();
    
    /// <inheritdoc cref="TestFixture.ShouldResetDatabase"/>
    public bool ShouldResetDatabase
    {
        get => fixture.ShouldResetDatabase;
        set => fixture.ShouldResetDatabase = value;
    }
    
    /// <inheritdoc />
    public virtual async ValueTask InitializeAsync()
    {
        DbContext.ChangeTracker.Clear();
        
        if (fixture.ShouldResetDatabase)
        {
            await fixture.Container.ResetDatabase();

            var seedData = EnumerateDefaultSeedData().Concat(EnumerateCustomSeedData());
            await AddDataAsync(seedData.Distinct().ToArray());
            fixture.ShouldResetDatabase = false;
        }
    }

    /// <inheritdoc />
    public virtual async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await HttpClient.SendAsync(new LogoutRequest());
        HttpClient.Dispose();
    }

    /// <summary>
    /// Enumerates the default seed data common to all tests.
    /// </summary>
    protected virtual IEnumerable<object> EnumerateDefaultSeedData()
    {
        yield return TestData.Lab;
        yield return TestData.AdminUser;
        yield return TestData.ManagerUser;
        yield return TestData.AnalystUser;
        yield return TestData.MemberUser;
        yield return TestData.NonMemberUser;
    }
    
    /// <summary>
    /// Enumerates the additional seed data for a specific class of tests.
    /// </summary>
    protected virtual IEnumerable<object> EnumerateCustomSeedData()
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
        await DbContext.SaveChangesAsync();
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
    protected Task<HttpResponseMessage> LoginAs(User user) => HttpClient.SendAsync(new LoginRequest
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
    protected Task<HttpResponseMessage> SetCurrentLab(Guid? labId) => HttpClient.SendAsync(new SetCurrentLaboratoryRequest
    {
        LaboratoryId = labId
    });
}
