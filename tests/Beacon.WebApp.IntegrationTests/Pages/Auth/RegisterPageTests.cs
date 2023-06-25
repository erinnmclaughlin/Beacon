using Beacon.Common.Requests.Auth;
using Beacon.WebApp.IntegrationTests.Http;
using BeaconUI.Core.Pages.Auth;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Pages.Auth;

public class RegisterPageTests : BeaconTestContext
{
    [Fact]
    public async Task WhenRegistrationIsSuccessful_LogInAndNavigateToHome()
    {
        // Arrange:
        SetupCoreServices();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Post, "/api/auth/register").ThenRespondOK(AuthHelper.DefaultSession);

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<RegisterPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        cut.WaitForAssertion(() => navManager.Uri.Should().Be(navManager.BaseUri));
    }

    [Fact]
    public async Task WhenRegistrationIsUnsuccessful_ShowError()
    {
        // Arrange:
        SetupCoreServices();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Post, "/api/auth/register").ThenRespondValidationProblem(new()
        {
            { nameof(RegisterRequest.EmailAddress), new[] { "Some error message" } }
        });

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<RegisterPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        cut.WaitForAssertion(() => cut.Find(".validation-message").TextContent.Should().Be("Some error message"));
    }
}
