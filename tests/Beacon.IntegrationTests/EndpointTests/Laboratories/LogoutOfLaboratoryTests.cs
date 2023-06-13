using Beacon.Common.Auth.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public sealed class LogoutOfLaboratoryTests : EndpointTestBase
{
    public LogoutOfLaboratoryTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task LogoutSucceeds()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        await client.PostAsync($"api/laboratories/{TestData.DefaultLaboratory.Id}/login", null);

        var response = await client.GetAsync("api/laboratories/logout");
        response.EnsureSuccessStatusCode();
    }
}
