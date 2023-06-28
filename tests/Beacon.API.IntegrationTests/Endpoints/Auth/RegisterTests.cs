using Beacon.Common.Requests.Auth;
using FluentValidation;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class RegisterTests : TestBase
{
    public RegisterTests(TestFixture testFixture) : base(testFixture)
    {
    }

    [Fact(DisplayName = "Register fails when required information is missing")]
    public async Task Register_FailsWhenRequiredInformationIsMissing()
    {
        await Assert.ThrowsAnyAsync<ValidationException>(() => SendAsync(new RegisterRequest()));
    }

    [Fact(DisplayName = "Register fails when email is already associated with an account")]
    public async Task Register_FailsWhenEmailExists()
    {
        await Assert.ThrowsAnyAsync<ValidationException>(() => SendAsync(new RegisterRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "something",
            DisplayName = "something"
        }));
    }

    [Fact(DisplayName = "Register succeeds when request is valid")]
    public async Task Register_SucceedsWhenRequestIsValid()
    {
        await SendAsync(new RegisterRequest
        {
            EmailAddress = "newuser@website.com",
            Password = "!!newuser",
            DisplayName = "New User"
        });
    }
}
