using Beacon.Common.Requests.Auth;
using BeaconUI.Core.Auth;
using ErrorOr;

namespace Beacon.WebApp.IntegrationTests.Pages.Auth;

[Trait("Feature", "User Registration & Login")]
public class RegisterPageTests : BeaconTestContext
{
    [Fact]
    public async Task WhenRegistrationIsSuccessful_LogInAndNavigateToHome()
    {
        MockApi.Succeeds<RegisterRequest>();

        var cut = RenderComponent<RegisterPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        cut.WaitForAssertion(() => NavigationManager.Uri.Should().Be(NavigationManager.BaseUri));
    }

    [Fact]
    public async Task WhenRegistrationIsUnsuccessful_ShowError()
    {
        MockApi.Fails<RegisterRequest>(Error.Validation(nameof(RegisterRequest.EmailAddress), "Some error message"));

        var cut = RenderComponent<RegisterPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        cut.WaitForAssertion(() => cut.Find(".validation-message").TextContent.Should().Be("Some error message"));
        NavigationManager.History.Should().BeEmpty();
    }
}
