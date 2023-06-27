using Beacon.Common.Requests.Auth;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class LoginTests : TestBase
{
    private readonly HttpClient _httpClient;

    public LoginTests(ApiFactory factory) : base(factory)
    {
        _httpClient = factory.CreateClient();
    }

    [Fact(DisplayName = "Login fails when required information is missing")]
    public async Task Login_FailsWhenRequiredInformationIsMissing()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Login fails when user does not exist")]
    public async Task Login_FailsWhenUserDoesNotExist()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "notreal@doesntexist.com",
            Password = "password123"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Login fails when password is incorrect")]
    public async Task Login_FailsWhenPasswordIsIncorrect()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "NOT!!admin"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Login succeeds when request is valid")]
    public async Task Login_SucceedsWhenRequestIsValid()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }
}
