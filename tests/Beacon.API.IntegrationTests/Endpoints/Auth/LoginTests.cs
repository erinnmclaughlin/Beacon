using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Collection(nameof(AuthTestFixture))]
public sealed class LoginTests : IAsyncLifetime
{
    private readonly AuthTestFixture _fixture;
    private readonly HttpClient _httpClient;

    public LoginTests(AuthTestFixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        if (await db.Database.EnsureCreatedAsync())
            db.AddTestData();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact(DisplayName = "Login succeeds when request is valid")]
    public async Task Login_SucceedsWhenRequestIsValid()
    {
        var response = await LoginAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }

    [Fact(DisplayName = "Login fails when required information is missing")]
    public async Task Login_FailsWhenRequiredInformationIsMissing()
    {
        var response = await LoginAsync(new LoginRequest());
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Login fails when user does not exist")]
    public async Task Login_FailsWhenUserDoesNotExist()
    {
        var response = await LoginAsync(new LoginRequest
        {
            EmailAddress = "notreal@doesntexist.com",
            Password = "password123"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "Login fails when password is incorrect")]
    public async Task Login_FailsWhenPasswordIsIncorrect()
    {
        var response = await LoginAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "NOT!!admin"
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }

    private async Task<HttpResponseMessage> LoginAsync(LoginRequest request)
    {
        return await _httpClient.PostAsJsonAsync("api/auth/login", request);
    }
}
