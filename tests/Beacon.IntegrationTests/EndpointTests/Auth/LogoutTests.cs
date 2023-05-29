using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LogoutTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public LogoutTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task Logout_ShouldSucceed()
    {
        await _factory.SeedDbWithUserData("test@test.com", "test", "pwd12345");

        // log in:
        await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "test@test.com",
            Password = "pwd12345"
        });

        // current user should be available after logging in:
        var currentUser = await _httpClient.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeTrue();

        // log out:
        var response = await _httpClient.GetAsync("api/auth/logout");

        response.IsSuccessStatusCode.Should().BeTrue();

        // current user should no longer be available after logging out:
        currentUser = await _httpClient.GetAsync("api/auth/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();
    }
}
