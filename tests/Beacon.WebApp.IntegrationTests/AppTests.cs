namespace Beacon.WebApp.IntegrationTests;

[Trait("Category", "User Registration & Login")]
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
        cut.WaitForAssertion(() => UrlShouldBe("login"), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void WebApp_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        this.SetAuthorized();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        NavigationManager.NavigateTo("");

        cut.WaitForElement("[data-test-id=\"profileDropdown\"]", TimeSpan.FromSeconds(5)).Click();
        cut.WaitForElement("[data-test-id=\"logoutButton\"]", TimeSpan.FromSeconds(5)).Click();

        // Assert
        cut.WaitForAssertion(() => UrlShouldBe("login"), TimeSpan.FromSeconds(5));
    }
}
