using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Enums;
using Beacon.Common.Laboratories.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class UpdateUserMembershipTests : EndpointTestBase
{
    public UpdateUserMembershipTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization();
    }

    [Fact]
    public async Task UpdateMembershipType_SucceedsWhenRequestIsValid()
    {
        var memberId = Guid.NewGuid();

        var client = CreateClient(db =>
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
            db.LaboratoryMemberships.Add(new LaboratoryMembership
            {
                LaboratoryId = TestData.DefaultLaboratory.Id,
                MemberId = memberId,
                MembershipType = LaboratoryMembershipType.Analyst
            });

            db.SaveChanges();
        });

        var uri = $"api/laboratories/{TestData.DefaultLaboratory.Id}/memberships/{memberId}/membershipType";
        var response = await client.PutAsJsonAsync(uri, new UpdateMembershipTypeRequest
        { 
            MembershipType = LaboratoryMembershipType.Manager 
        }, JsonSerializerOptions);

        var labDetails = await client.GetFromJsonAsync<LaboratoryDetailDto>($"api/laboratories/{TestData.DefaultLaboratory.Id}", JsonSerializerOptions);
        labDetails!.Members.First(m => m.Id == memberId).MembershipType.Should().Be(LaboratoryMembershipType.Manager);
    }
}
