using Beacon.Common.Auth.Register;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

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
}
