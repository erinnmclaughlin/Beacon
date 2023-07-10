using Beacon.API.Persistence;
using Beacon.Common.Services;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Trait("Feature", "User Registration & Login")]
public sealed class GetCurrentUserTests : TestBase
{
    public GetCurrentUserTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[008] Get current user returns 401 if user is not logged in")]
    public async Task GetCurrentUser_Returns401_WhenNotLoggedIn()
    {
        RunAsAnonymous();

        var response = await GetAsync("api/users/current");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "[008] Get current user returns logged in user")]
    public async Task GetCurrentUser_ReturnsExpectedResult_WhenUserIsLoggedIn()
    {
        RunAsAdmin();

        var response = await GetAsync("api/users/current");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var session = await DeserializeAsync<SessionContext>(response);

        Assert.NotNull(session);
        Assert.Equal(TestData.AdminUser.Id, session.CurrentUser.Id);
        Assert.Equal(TestData.AdminUser.DisplayName, session.CurrentUser.DisplayName);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.Users.Add(TestData.AdminUser);
        db.SaveChanges();
    }
}
