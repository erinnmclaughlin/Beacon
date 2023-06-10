using Beacon.Common.Auth.Requests;
using Beacon.WebApp.IntegrationTests.Http;
using BeaconUI.Core;
using BeaconUI.Core.Pages.Auth;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Auth.Pages;

public class LoginPageTests : BeaconTestContext
{
    [Fact]
    public async Task GivenValidCredentials_WhenLoginIsClicked_ThenRedirectToHome()
    {
        // Arrange:
        SetupCoreServices();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Post, "/api/auth/login").ThenRespondOK(AuthHelper.DefaultUser);

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        cut.WaitForAssertion(() => navManager.Uri.Should().Be(navManager.BaseUri));
    }

    [Fact]
    public async Task GivenInvalidCredentials_WhenLoginIsClicked_ThenDisplayError()
    {
        // Arrange:
        SetupCoreServices();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Post, "/api/auth/login").ThenRespondValidationProblem(new()
        {
            { nameof(LoginRequest.EmailAddress), new[] { "Some error message" } }
        });

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        navManager.History.Should().BeEmpty();
        cut.WaitForAssertion(() => cut.Find(".validation-message").TextContent.Should().Be("Some error message"));
    }
}