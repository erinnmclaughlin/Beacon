using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Auth;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

[Collection("LaboratoryTests")]
public class LoginToLaboratoryTests : EndpointTestBase
{
    public LoginToLaboratoryTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task LoginToLab_FailsWhenUserIsNotLabMember()
    {
        var labId = Guid.NewGuid();

        var client = CreateClient(async db =>
        {
            db.Add(Laboratory.CreateNew(labId, "Lab 2", new User 
            {
                Id = Guid.NewGuid(),
                DisplayName = "other user",
                EmailAddress = "other@test.com",
                HashedPassword = new PasswordHasher().Hash("testing123", out var salt),
                HashedPasswordSalt = salt
            }));

            await db.SaveChangesAsync();
        });

        await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        var response = await client.PostAsync($"api/laboratories/{labId}/login", null);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task LoginToLab_SucceedsWhenUserIsLabMember()
    {
        var client = CreateClient();

        await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        var response = await client.PostAsync($"api/laboratories/{TestData.DefaultLaboratory.Id}/login", null);
        response.EnsureSuccessStatusCode();
    }
}
