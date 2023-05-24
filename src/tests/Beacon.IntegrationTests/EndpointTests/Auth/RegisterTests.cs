using Beacon.Common.Auth.Register;
using System.Net;
using System.Net.Http.Json;

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
}
