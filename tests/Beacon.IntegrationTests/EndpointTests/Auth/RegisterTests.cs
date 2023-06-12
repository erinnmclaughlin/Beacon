using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class RegisterTests : EndpointTestBase
{
    public RegisterTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_ShouldFail_IfEmailIsMissing()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "",
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Theory]
    [InlineData("nope")]
    [InlineData("nope@")]
    [InlineData("@nope")]
    [InlineData("@nope@")]
    public async Task Register_ShouldFail_IfEmailIsInvalid(string invalidEmail)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = invalidEmail,
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfPasswordIsMissing()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "someValidName",
            Password = ""
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfDisplayNameIsMissing()
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "",
            Password = "someValidPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldFail_IfEmailIsTaken()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Register_ShouldSucceed_WhenRequestIsValid()
    {
        var client = CreateClient();

        // getting current user should fail if we're not logged in:
        var currentUser = await client.GetAsync("api/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();

        // register:
        var response = await client.PostAsJsonAsync("api/auth/register", new RegisterRequest
        {
            EmailAddress = "someValidEmail@website.com",
            DisplayName = "someValidName",
            Password = "someValidPassword"
        });

        // check that register was successful:
        response.EnsureSuccessStatusCode();

        // check that auth cookie was included in the response:
        response.Headers.Contains("Set-Cookie");

        // try getting current user again; this time response should be successful:
        currentUser = await client.GetAsync("api/me");
        currentUser.EnsureSuccessStatusCode();
    }
}
