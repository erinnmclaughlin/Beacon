using Beacon.Common.Requests.Auth;
using System.Net;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class GetCurrentUserTests : TestBase
{
    public GetCurrentUserTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Get current user returns 401 if user is not logged in")]
    public async Task GetCurrentUser_Returns401_WhenNotLoggedIn()
    {
        SetCurrentUser(Guid.Empty);

        var response = await _httpClient.GetAsync("api/users/current");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Get current user returns logged in user")]
    public async Task GetCurrentUser_ReturnsExpectedResult_WhenUserIsLoggedIn()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var currentUser = await SendAsync(new GetCurrentUserRequest());

        Assert.Equal(TestData.AdminUser.Id, currentUser.Id);
        Assert.Equal(TestData.AdminUser.DisplayName, currentUser.DisplayName);
    }
}
