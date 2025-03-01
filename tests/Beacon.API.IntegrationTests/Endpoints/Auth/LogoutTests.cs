﻿using Beacon.API.Persistence;
using Beacon.Common.Requests.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Auth;

[Trait("Feature", "User Registration & Login")]
public sealed class LogoutTests : IClassFixture<AuthTestFixture>
{
    private readonly HttpClient _httpClient;

    public LogoutTests(AuthTestFixture fixture)
    {
        _httpClient = fixture.CreateClient();

        using var scope = fixture.Services.CreateScope();
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
        await LoginRequest.SendAsync(_httpClient, new LoginRequest
        {
            EmailAddress = TestData.AdminUser.EmailAddress,
            Password = "!!admin"
        });

        await AssertGetCurrentUserStatus(HttpStatusCode.OK);

        await LogoutRequest.SendAsync(_httpClient, new());

        await AssertGetCurrentUserStatus(HttpStatusCode.Unauthorized);
    }

    private async Task AssertGetCurrentUserStatus(HttpStatusCode expectedStatusCode)
    {
        var response = await GetSessionContextRequest.SendAsync(_httpClient, new());
        Assert.Equal(expectedStatusCode, response.StatusCode);
    }
}
