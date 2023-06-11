﻿using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Auth;

public class LogoutTests : EndpointTestBase
{
    private readonly HttpClient _httpClient;

    public LogoutTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        _httpClient = CreateClient();
    }

    [Fact]
    public async Task Logout_ShouldSucceed()
    {
        // log in:
        await _httpClient.PostAsJsonAsync("portal/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        // current user should be available after logging in:
        var currentUser = await _httpClient.GetAsync("portal/me");
        currentUser.IsSuccessStatusCode.Should().BeTrue();

        // log out:
        var response = await _httpClient.GetAsync("portal/logout");

        response.IsSuccessStatusCode.Should().BeTrue();

        // current user should no longer be available after logging out:
        currentUser = await _httpClient.GetAsync("portal/me");
        currentUser.IsSuccessStatusCode.Should().BeFalse();
    }
}
