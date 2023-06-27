using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class LogoutTests : IClassFixture<WebApplicationFactory<BeaconWebHost>>
{
    private readonly WebApplicationFactory<BeaconWebHost> _factory;
    private readonly HttpClient _httpClient;

    public LogoutTests(WebApplicationFactory<BeaconWebHost> factory)
    {
        _factory = factory.WithWebHostBuilder(b => b.ConfigureBeaconTestServices());
        _httpClient = _factory.CreateClient();

        ResetState();
    }

    [Fact(DisplayName = "Logout succeeds when user is logged in")]
    public async Task Logout_SucceedsWhenUserIsLoggedIn()
    {
        await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        var response = await _httpClient.GetAsync("api/auth/logout");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    private void ResetState()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        if (dbContext.Database.EnsureCreated())
        {
            dbContext.Users.Add(TestData.AdminUser);
            dbContext.SaveChanges();
        }

        dbContext.ChangeTracker.Clear();
    }
}
