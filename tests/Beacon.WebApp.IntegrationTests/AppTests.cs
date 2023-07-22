using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;

namespace Beacon.WebApp.IntegrationTests;

[Trait("Feature", "User Registration & Login")]
public class AppTests : BeaconTestContext
{
    [Fact]
    public void WebApp_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange
        MockApi.Fails<GetSessionContextRequest, SessionContext>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        NavigationManager.NavigateTo("");

        // Assert
        cut.WaitForAssertion(() => UrlShouldBe("login"));
    }

    [Fact]
    public void WebApp_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        MockApi.Succeeds<GetSessionContextRequest, SessionContext>(AuthHelper.DefaultSession);
        MockApi.Succeeds<LogoutRequest>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        NavigationManager.NavigateTo("");

        cut.WaitForElement("[data-test-id=\"profileDropdown\"]").Click();
        cut.WaitForElement("[data-test-id=\"logoutButton\"]").Click();

        // Assert
        cut.WaitForAssertion(() => UrlShouldBe("login"));
    }
}
