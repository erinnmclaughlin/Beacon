using Beacon.Common.Models;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

public sealed class GetCurrentUserTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public GetCurrentUserTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Get current user returns unauthroized when user is not logged in")]
    public async Task GetCurrentUser_ReturnsUnauthorized_WhenUserIsNotLoggedIn()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("api/users/current");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact(DisplayName = "Get current user returns logged in user")]
    public async Task GetCurrentUser_ReturnsExpectedResult_WhenUserIsLoggedIn()
    {
        var client = _factory.CreateClient(TestData.AdminUser.Id);        
        var response = await client.GetAsync("api/users/current");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var userData = await response.Content.ReadFromJsonAsync<CurrentUserDto>();

        Assert.Equal(TestData.AdminUser.Id, userData?.Id);
        Assert.Equal(TestData.AdminUser.DisplayName, userData?.DisplayName);
    }
}
