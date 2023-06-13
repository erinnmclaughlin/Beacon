using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LoginTests : EndpointTestBase
{
    private readonly HttpClient _httpClient;

    public LoginTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        _httpClient = CreateClient();
    }

    [Fact]
    public async Task Login_ShouldFail_WhenUserDoesNotExist()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "nobody@invalid.net",
            Password = "pwd12345"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_ShouldFail_WhenPasswordIsInvalid()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = "not" + TestData.DefaultPassword // an invalid password
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenCredentialsAreValid()
    {
        // getting current user should fail if we're not logged in:
        var currentUser = await _httpClient.GetAsync("api/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();

        // log in:
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        // check that login was successful:
        response.EnsureSuccessStatusCode();

        // check that auth cookie was included in the response:
        response.Headers.Contains("Set-Cookie");

        // try getting current user again; this time response should be successful:
        currentUser = await _httpClient.GetAsync("api/me");
        currentUser.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
