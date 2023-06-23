using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Auth;
using Beacon.Common.Memberships;

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
            var otherUser = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "other user",
                EmailAddress = "other@test.com",
                HashedPassword = new PasswordHasher().Hash("testing123", out var salt),
                HashedPasswordSalt = salt
            };

            var lab = new Laboratory { Id = labId, Name = "Lab 2" };
            lab.AddMember(otherUser.Id, LaboratoryMembershipType.Admin);

            db.AddRange(otherUser, lab);
            await db.SaveChangesAsync();
        });

        await client.PostAsJsonAsync("api/auth/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        var response = await client.PostAsync($"api/laboratories/{labId}/login", null);
        response.IsSuccessStatusCode.Should().BeFalse();
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
