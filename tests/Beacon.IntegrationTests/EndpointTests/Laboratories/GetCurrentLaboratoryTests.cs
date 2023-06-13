using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public sealed class GetCurrentLaboratoryTests : EndpointTestBase
{
    public GetCurrentLaboratoryTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetCurrentLabFails_WhenUserIsNotLoggedInToLab()
    {
        var client = CreateClient();

        await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        var response = await client.GetAsync("api/laboratories/current");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetCurrentLabSucceeds_WhenUserIsLoggedIntoLab()
    {
        var client = CreateClient();

        await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        await client.PostAsync($"api/laboratories/{TestData.DefaultLaboratory.Id}/login", null);

        var response = await client.GetAsync("api/laboratories/current");
        response.EnsureSuccessStatusCode();
    }
}
