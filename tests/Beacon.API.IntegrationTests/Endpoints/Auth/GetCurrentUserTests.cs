using Beacon.API.Persistence;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class GetCurrentUserTests : TestBase
{
    public GetCurrentUserTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Get current user returns 401 if user is not logged in")]
    public async Task GetCurrentUser_Returns401_WhenNotLoggedIn()
    {
        SetCurrentUser(Guid.Empty);

        var response = await GetAsync("api/users/current");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Get current user returns logged in user")]
    public async Task GetCurrentUser_ReturnsExpectedResult_WhenUserIsLoggedIn()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync("api/users/current");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var currentUser = await DeserializeAsync<CurrentUserDto>(response);

        Assert.NotNull(currentUser);
        Assert.Equal(TestData.AdminUser.Id, currentUser.Id);
        Assert.Equal(TestData.AdminUser.DisplayName, currentUser.DisplayName);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.Users.Add(TestData.AdminUser);
        db.SaveChanges();
    }
}
