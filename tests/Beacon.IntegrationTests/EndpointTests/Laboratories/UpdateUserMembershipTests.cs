using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class UpdateUserMembershipTests : EndpointTestBase
{
    public UpdateUserMembershipTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpdateMembershipType_FailsWhenMemberIsInvalid()
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);

        var uri = $"api/members/{Guid.NewGuid()}/membershipType";
        var response = await CreateClient().PutAsJsonAsync(uri, new UpdateMembershipTypeRequest
        {
            MembershipType = LaboratoryMembershipType.Manager
        }, JsonSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
        });

        var uri = $"api/members/{memberId}/membershipType";
        var response = await client.PutAsJsonAsync(uri, new UpdateMembershipTypeRequest
        { 
            MembershipType = LaboratoryMembershipType.Manager 
        }, JsonSerializerOptions);

        response.EnsureSuccessStatusCode();
    }
}
