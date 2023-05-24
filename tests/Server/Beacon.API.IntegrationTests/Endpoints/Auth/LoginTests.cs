using Beacon.Common.Auth.Login;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public class LoginTests : IClassFixture<BeaconTestApplicationFactory>
{
    private readonly BeaconTestApplicationFactory _factory;

    public LoginTests(BeaconTestApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ShouldFail_WhenUserDoesNotExist()
    {
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "nobody@invalid.net",
            Password = "pwd12345"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_ShouldFail_WhenPasswordIsInvalid()
    {
        await _factory.ResetTestDb();
        await _factory.AddUser("test@test.com", "test", "pwd12345");
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "test@test.com",
            Password = "pwd123456"
        });

        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Login_ShouldSucceed_WhenCredentialsAreValid()
    {
        await _factory.ResetTestDb();
        await _factory.AddUser("test@test.com", "test", "pwd12345");
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = "test@test.com",
            Password = "pwd12345"
        });

        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
