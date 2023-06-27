using Beacon.Common.Requests.Auth;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class RegisterTests : TestBase
{
    private readonly HttpClient _httpClient;

    public RegisterTests(ApiFactory factory) : base(factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact(DisplayName = "Register fails when required information is missing")]
    public async Task Register_FailsWhenRequiredInformationIsMissing()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Register fails when email is already associated with an account")]
    public async Task Register_FailsWhenEmailExists()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "something",
            DisplayName = "something"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Register succeeds when request is valid")]
    public async Task Register_SucceedsWhenRequestIsValid()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "newuser@website.com",
            Password = "!!newuser",
            DisplayName = "New User"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
