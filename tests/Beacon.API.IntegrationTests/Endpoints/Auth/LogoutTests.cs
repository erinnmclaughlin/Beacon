using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Trait("Feature", "User Registration & Login")]
public sealed class LogoutTests : IClassFixture<AuthTestFixture>
{
    private readonly AuthTestFixture _fixture;
    private readonly HttpClient _httpClient;

    public LogoutTests(AuthTestFixture fixture)
    {
        _fixture = fixture;
        _httpClient = fixture.CreateClient();

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        if (db.Database.EnsureCreated())
        {
            db.Users.Add(TestData.AdminUser);
            db.SaveChanges();
        }
    }

    [Fact(DisplayName = "[008] Logged in user can sucessfully log out")]
    public async Task LoggedInUserCanSuccessfullyLogOut()
    {
        await _httpClient.SendAsync(new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        await AssertGetCurrentUserStatus(HttpStatusCode.OK);

        await _httpClient.SendAsync(new LogoutRequest());

        await AssertGetCurrentUserStatus(HttpStatusCode.Unauthorized);
    }

    private async Task AssertGetCurrentUserStatus(HttpStatusCode expectedStatusCode)
    {
        var response = await _httpClient.SendAsync(new GetSessionContextRequest());
        Assert.Equal(expectedStatusCode, response.StatusCode);
    }
}
