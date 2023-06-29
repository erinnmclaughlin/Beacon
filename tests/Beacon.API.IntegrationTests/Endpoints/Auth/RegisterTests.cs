using Beacon.Common.Requests.Auth;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class RegisterTests : TestBase
{
    public RegisterTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Register succeeds when request is valid")]
    public async Task Register_SucceedsWhenRequestIsValid()
    {
        var response = await PostAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "newuser@website.com",
            Password = "!!newuser",
            DisplayName = "New User"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact(DisplayName = "Register fails when required information is missing")]
    public async Task Register_FailsWhenRequiredInformationIsMissing()
    {
        var response = await PostAsync("api/auth/register", new RegisterRequest());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Register fails when email is already associated with an account")]
    public async Task Register_FailsWhenEmailExists()
    {
        var response = await PostAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "something",
            DisplayName = "something"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
}
