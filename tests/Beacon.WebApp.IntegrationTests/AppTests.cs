using Beacon.WebApp.IntegrationTests.Auth;
using BeaconUI.Core.Helpers;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests;

public class AppTests : TestContext
{
    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange
        this.AddTestAuthorization().SetNotAuthorized();
        Services.AddBeaconUI();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/auth/me").ThenRespondNotFound();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }

    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        this.AddTestAuthorization().SetAuthorized("Test");
        Services.AddBeaconUI();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/auth/me").ThenRespondOK(AuthHelper.DefaultUser);
        mockHttp.When(HttpMethod.Get, "/api/auth/logout").ThenRespondNoContent();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        cut.WaitForElement("button#logout").Click();

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }
}
