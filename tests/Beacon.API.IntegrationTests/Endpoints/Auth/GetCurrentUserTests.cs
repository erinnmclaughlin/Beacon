using Beacon.Common.Requests.Auth;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class GetCurrentUserTests : TestBase
{
    [Fact(DisplayName = "Get current user returns logged in user")]
    public async Task GetCurrentUser_ReturnsExpectedResult_WhenUserIsLoggedIn()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var currentUser = await SendAsync(new GetCurrentUserRequest());

        Assert.Equal(TestData.AdminUser.Id, currentUser.Id);
        Assert.Equal(TestData.AdminUser.DisplayName, currentUser.DisplayName);
    }
}
