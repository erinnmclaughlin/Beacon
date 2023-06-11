using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Laboratories.Enums;
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

        var uri = $"api/memberships/{Guid.NewGuid()}/membershipType";
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

        var uri = $"api/memberships/{memberId}/membershipType";
        var response = await client.PutAsJsonAsync(uri, new UpdateMembershipTypeRequest
        { 
            MembershipType = LaboratoryMembershipType.Manager 
        }, JsonSerializerOptions);

        response.EnsureSuccessStatusCode();
    }
}
