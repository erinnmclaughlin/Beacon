namespace Beacon.WebApp.IntegrationTests;

[Trait("Feature", "User Registration & Login")]
public class AppTests : BeaconTestContext
{
    [Fact]
    public void WebApp_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange
        this.SetNotAuthorized();

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
        this.SetAuthorized();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        NavigationManager.NavigateTo("");

        cut.WaitForElement("[data-test-id=\"profileDropdown\"]").Click();
        cut.WaitForElement("[data-test-id=\"logoutButton\"]").Click();

        // Assert
        cut.WaitForAssertion(() => UrlShouldBe("login"));
    }
}
