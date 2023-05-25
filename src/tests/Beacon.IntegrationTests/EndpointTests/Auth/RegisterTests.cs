using Beacon.Common.Auth.Register;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class RegisterTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;
    private readonly HttpClient _httpClient;

    public RegisterTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldFail_IfEmailIsTaken()
    {
        await _factory.SeedDbWithUserData("test@test.com", "test", "pwd12345");

        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "test@test.com",
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfEmailIsMissing()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "",
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineData("nope")]
    [InlineData("nope@")]
    [InlineData("@nope")]
    [InlineData("@nope@")]
    public async Task Register_ShouldFail_IfEmailIsMissingOrInvalid(string invalidEmail)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = invalidEmail,
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfPasswordIsMissing()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "someValidName",
            Password = ""
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfDisplayNameIsMissing()
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "",
            Password = "someValidPassword"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldSucceed_WhenRequestIsValid()
    {
        // getting current user should fail if we're not logged in:
        var currentUser = await _httpClient.GetAsync("api/users/current");
        currentUser.IsSuccessStatusCode.Should().BeFalse();

        // register:
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        // check that register was successful:
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // check that auth cookie was included in the response:
        response.Headers.Contains("Set-Cookie");

        // try getting current user again; this time response should be successful:
        currentUser = await _httpClient.GetAsync("api/users/current");
        currentUser.IsSuccessStatusCode.Should().BeTrue();
    }
}
