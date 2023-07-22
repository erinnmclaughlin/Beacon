using Beacon.Common.Requests.Auth;
using Beacon.Common.Services;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

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
        MockApi.Fails<GetSessionContextRequest, SessionContext>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        NavigationManager.NavigateTo("");

        // Assert
        cut.WaitForAssertion(() => NavigationManager.Uri.Should().Be($"{NavigationManager.BaseUri}login"));
    }

    [Fact]
    public void WebApp_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        SetupCoreServices();
        Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

        MockApi.Succeeds<GetSessionContextRequest, SessionContext>(AuthHelper.DefaultSession);
        MockApi.Succeeds<LogoutRequest>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        NavigationManager.NavigateTo("");

        cut.WaitForElement("[data-test-id=\"profileDropdown\"]").Click();
        cut.WaitForElement("[data-test-id=\"logoutButton\"]").Click();

        // Assert
        cut.WaitForAssertion(() => NavigationManager.Uri.Should().Be($"{NavigationManager.BaseUri}login"));
    }

    protected override void Initialize()
    {
    }
}
