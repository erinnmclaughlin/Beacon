using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Auth;
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
            await fixture.ResetDatabase(EnumerateSeedData().ToArray());
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
    /// Log in as a specific user.
    /// </summary>
    /// <param name="user">The user to log in as.</param>
    protected Task<HttpResponseMessage> LoginAs(User user) => HttpClient.SendAsync(new LoginRequest
    {
        EmailAddress = user.EmailAddress,
        Password = $"!!{user.DisplayName.ToLower()}"
    });
}
