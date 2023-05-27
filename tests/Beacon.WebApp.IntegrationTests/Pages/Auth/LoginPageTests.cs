using Beacon.Common.Auth;
using Beacon.Common.Auth.Login;
using BeaconUI.Core.Auth;
using BeaconUI.Core.Pages.Auth;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Pages.Auth;

public class LoginPageTests : TestContext
{
    [Fact]
    public async Task GivenValidCredentials_WhenLoginIsClicked_ThenRedirectToHome()
    {
        // Arrange:
        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Post, "/api/auth/login").ThenRespondOK(new UserDto
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@test.com",
            DisplayName = "test"
        });

        Services.AddScoped<BeaconAuthClient>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        navManager.History.Should().ContainSingle().Which.Uri.Should().Be("");
    }

    [Fact]
    public async Task GivenInvalidCredentials_WhenLoginIsClicked_ThenDisplayError()
    {
        // Arrange:
        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Post, "/api/auth/login").ThenRespondValidationProblem(new()
        {
            { nameof(LoginRequest.EmailAddress), new[] { "Some error message" } }
        });

        Services.AddScoped<BeaconAuthClient>();
        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        navManager.History.Should().BeEmpty();
        cut.Find(".validation-message").TextContent.Should().Be("Some error message");
    }
}