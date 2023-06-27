using Beacon.Common.Models;
using Beacon.Common.Requests.Auth;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class GetCurrentUserTests : TestBase
{
    public GetCurrentUserTests(ApiFactory factory) : base(factory)
    {
    }

    [Fact(DisplayName = "Get current user returns logged in user")]
    public async Task GetCurrentUser_ReturnsExpectedResult_WhenUserIsLoggedIn()
    {
        var response = await _httpClient.GetAsync("api/users/current");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var userData = await response.Content.ReadFromJsonAsync<CurrentUserDto>();

        Assert.Equal(TestData.ManagerUser.Id, userData?.Id);
        Assert.Equal(TestData.ManagerUser.DisplayName, userData?.DisplayName);
    }
}
