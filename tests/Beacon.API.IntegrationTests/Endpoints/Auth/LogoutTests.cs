using Beacon.Common.Requests.Auth;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class LogoutTests : TestBase
{
    private readonly HttpClient _httpClient;

    public LogoutTests(ApiFactory factory) : base(factory)
    {
        _httpClient = factory.CreateClient();
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
}
