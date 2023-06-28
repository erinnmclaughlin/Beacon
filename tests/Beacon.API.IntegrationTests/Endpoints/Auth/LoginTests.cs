using Beacon.Common.Requests.Auth;
using FluentValidation;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class LoginTests : TestBase
{
    public LoginTests(TestFixture testFixture) : base(testFixture)
    {
    }

    [Fact(DisplayName = "Login fails when required information is missing")]
    public async Task Login_FailsWhenRequiredInformationIsMissing()
    {
        await Assert.ThrowsAnyAsync<ValidationException>(() => SendAsync(new LoginRequest()));
    }

    [Fact(DisplayName = "Login fails when user does not exist")]
    public async Task Login_FailsWhenUserDoesNotExist()
    {
        await Assert.ThrowsAnyAsync<ValidationException>(() => SendAsync(new LoginRequest
        {
            EmailAddress = "notreal@doesntexist.com",
            Password = "password123"
        }));
    }

    [Fact(DisplayName = "Login fails when password is incorrect")]
    public async Task Login_FailsWhenPasswordIsIncorrect()
    {
        await Assert.ThrowsAnyAsync<ValidationException>(() => SendAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "NOT!!admin"
        }));
    }

    [Fact(DisplayName = "Login succeeds when request is valid")]
    public async Task Login_SucceedsWhenRequestIsValid()
    {
        await SendAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });
    }
}
