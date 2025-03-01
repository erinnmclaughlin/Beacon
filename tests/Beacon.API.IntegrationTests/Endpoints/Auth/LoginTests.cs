using Beacon.Common.Requests.Auth;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Trait("Feature", "User Registration & Login")]
public sealed class LoginTests(TestFixture fixture) : TestBase(fixture)
{
    [Fact(DisplayName = "[008] Login succeeds when request is valid")]
    public async Task Login_SucceedsWhenRequestIsValid()
    {
        var response = await SendAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact(DisplayName = "[008] Login fails when required information is missing")]
    public async Task Login_FailsWhenRequiredInformationIsMissing()
    {
        var response = await SendAsync(new LoginRequest());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "[008] Login fails when user does not exist")]
    public async Task Login_FailsWhenUserDoesNotExist()
    {
        var response = await SendAsync(new LoginRequest
        {
            EmailAddress = "notreal@doesntexist.com",
            Password = "password123"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "[008] Login fails when password is incorrect")]
    public async Task Login_FailsWhenPasswordIsIncorrect()
    {
        var response = await SendAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "NOT!!admin"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }
}
