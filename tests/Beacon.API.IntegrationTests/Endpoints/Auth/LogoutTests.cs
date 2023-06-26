using Beacon.Common.Requests.Auth;
using Beacon.WebHost;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Collection(AuthTests.Name)]
public sealed class LogoutTests : TestBase
{
    public LogoutTests(DbContextFixture db, WebApplicationFactory<BeaconWebHost> factory) 
        : base(db, factory) { }

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
