using Beacon.WebApp.IntegrationTests.Http;
using Beacon.WebApp.IntegrationTests.Pages;
using BeaconUI.Core.Common.Auth;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests;

[Trait("Feature", "User Registration & Login")]
public class AppTests : BeaconTestContext
{
    [Fact]
    public void WebApp_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange
        this.AddTestAuthorization().SetNotAuthorized();
        SetupCoreServices();

        Services.AddMockHttpClient()
            .When(HttpMethod.Get, "/api/users/current")
            .ThenRespondNotFound();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }

    [Fact]
    public void WebApp_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        SetupCoreServices();
        Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/users/current").ThenRespondOK(AuthHelper.DefaultSession);
        mockHttp.When(HttpMethod.Get, "/api/auth/logout").ThenRespondNoContent();

        var authProvider = Services.GetRequiredService<BeaconAuthStateProvider>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        cut.WaitForElement("[data-test-id=\"profileDropdown\"]").Click();
        cut.WaitForElement("[data-test-id=\"logoutButton\"]").Click();

        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }
}
