using Beacon.Common.Requests.Auth;
using BeaconUI.Core.Auth;
using ErrorOr;

namespace Beacon.WebApp.IntegrationTests.Pages.Auth;

[Trait("Feature", "User Registration & Login")]
public class LoginPageTests : BeaconTestContext
{
    [Fact]
    public async Task GivenValidCredentials_WhenLoginIsClicked_ThenRedirectToHome()
    {
        MockApi.Succeeds<LoginRequest>();

        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        cut.WaitForAssertion(() => NavigationManager.Uri.Should().Be(NavigationManager.BaseUri));
    }

    [Fact]
    public async Task GivenInvalidCredentials_WhenLoginIsClicked_ThenDisplayError()
    {
        MockApi.Fails<LoginRequest>(Error.Validation(nameof(LoginRequest.EmailAddress), "Some error message"));

        var cut = RenderComponent<LoginPage>();

        // Act:
        cut.Find("input[type=email]").Change("test@test.com");
        cut.Find("input[type=password]").Change("password123");
        await cut.Find("form").SubmitAsync();

        // Assert:
        cut.WaitForAssertion(() => cut.Find(".validation-message").TextContent.Should().Be("Some error message"));
        NavigationManager.History.Should().BeEmpty();
    }
}