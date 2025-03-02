using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints;

public abstract class IntegrationTestBase(TestFixture fixture) : IAsyncLifetime, IClassFixture<TestFixture>
{
    /// <summary>
    /// A reference to the <see cref="BeaconDbContext"/> in the current test scope.
    /// </summary>
    protected BeaconDbContext DbContext => Scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
    
    /// <summary>
    /// An <see cref="HttpClient"/> instance configured to call the web API.
    /// </summary>
    protected HttpClient HttpClient { get; } = fixture.CreateClient();
    
    /// <summary>
    /// The service scope that the test is currently running in.
    /// </summary>
    protected IServiceScope Scope { get; } = fixture.Services.CreateScope();

    /// <summary>
    /// When <see langword="true"/>, the database will be reset to the checkpoint defined in the <see cref="fixture"/>.
    /// Seed data defined in <see cref="EnumerateSeedData"/> will be re-applied after the database is reset.
    /// </summary>
    protected bool ShouldResetDatabase
    {
        get => fixture[nameof(ShouldResetDatabase)] is null or true;
        set => fixture[nameof(ShouldResetDatabase)] = value;
    }
    
    /// <inheritdoc />
    public virtual async Task InitializeAsync()
    {
        if (ShouldResetDatabase)
        {
            await fixture.ResetDatabase();
            await AddSeedDataAsync(EnumerateSeedData().Append(TestData.Lab).ToArray());
            ShouldResetDatabase = false;
        }
    }

    /// <inheritdoc />
    public virtual Task DisposeAsync()
    {
        HttpClient.Dispose(); 
        Scope.Dispose();
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Defines the seed data to add to the database whenever it's reset.
    /// </summary>
    /// <returns>An <see cref="IEnumerable{T}"/> of data to add to the database.</returns>
    protected virtual IEnumerable<object> EnumerateSeedData()
    {
        yield break;
    }

    /// <summary>
    /// Adds the specified <paramref name="data"/> to the database.
    /// </summary>
    /// <param name="data">The data to add.</param>
    protected async Task AddSeedDataAsync(params object[] data)
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
    protected async Task<HttpResponseMessage> LoginAs(User user)
    {
        await HttpClient.SendAsync(new LogoutRequest());
        return await HttpClient.SendAsync(new LoginRequest
        {
            EmailAddress = user.EmailAddress,
            Password = $"!!{user.DisplayName.ToLower()}"
        });
    }

    /// <summary>
    /// Log in as a specified user and set the current lab to <see cref="TestData.Lab"/>.
    /// </summary>
    /// <param name="user">The user to log in as.</param>
    protected async Task LogInToDefaultLab(User user)
    {
        var loginResponse = await LoginAs(user);
        loginResponse.EnsureSuccessStatusCode();
        
        var setCurrentLabResponse = await SetCurrentLab(TestData.Lab.Id);
        setCurrentLabResponse.EnsureSuccessStatusCode();
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
