using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Memberships;

namespace Beacon.IntegrationTests.EndpointTests.Members;

[Collection("MembershipTests")]
public class UpdateUserMembershipTests : EndpointTestBase
{
    public UpdateUserMembershipTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateMembershipType_SucceedsWhenRequestIsValid()
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);

        var memberId = Guid.NewGuid();

        var client = CreateClient(async db =>
        {
            var member = new User
            {
                Id = memberId,
                DisplayName = "Member",
                EmailAddress = "member@membership.com",
                HashedPassword = new PasswordHasher().Hash("testing", out var salt),
                HashedPasswordSalt = salt
            };

            db.Users.Add(member);
            db.Memberships.Add(new Membership
            {
                LaboratoryId = TestData.DefaultLaboratory.Id,
                MemberId = memberId,
                MembershipType = LaboratoryMembershipType.Analyst
            });

            await db.SaveChangesAsync();
        }).AddLabHeader();

        var response = await client.PutAsJsonAsync("api/memberships", new UpdateMembershipRequest
        {
            MemberId = memberId,
            MembershipType = LaboratoryMembershipType.Manager
        }, JsonSerializerOptions);

        response.EnsureSuccessStatusCode();
    }
}
