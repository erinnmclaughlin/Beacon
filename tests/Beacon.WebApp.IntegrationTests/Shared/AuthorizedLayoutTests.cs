using Beacon.Common.Auth;
using BeaconUI.Core.Auth;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Shared;

public class AuthorizedLayoutTests : TestContext
{
    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenUserIsNotAuthorized()
    {
        // Arrange
        var authContext = this.AddTestAuthorization().SetNotAuthorized();
        Services.AddScoped<BeaconAuthClient>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        // Assert
        navManager.History.Last().Uri.Should().Be("login");
    }

    [Fact]
    public void AuthorizedLayout_RedirectsToLogin_WhenLoggedInUserClicksLogout()
    {
        // Arrange
        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, "/api/auth/logout").ThenReturnNoContent();

        var authContext = this.AddTestAuthorization().SetAuthorized("Test");
        Services.AddScoped<BeaconAuthClient>();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();

        // Act
        var cut = RenderComponent<BeaconUI.WebApp.App>();
        navManager.NavigateTo("");

        cut.WaitForElement("button#logout").Click();
        
        // Assert
        cut.WaitForAssertion(() => navManager.Uri.Should().Be($"{navManager.BaseUri}login"));
    }
}
