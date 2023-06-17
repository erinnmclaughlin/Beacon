using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryInvitationTests : EndpointTestBase
{
    public CreateLaboratoryInvitationTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Theory]
    [InlineData(LaboratoryMembershipType.Admin)]
    [InlineData(LaboratoryMembershipType.Manager)]
    public async Task InviteMember_Succeeds_WhenCurrentUserIsAdminOrManager(LaboratoryMembershipType membershipType)
    {
        AddTestAuthorization(membershipType);

        var client = CreateClient();

        var request = new InviteLabMemberRequest
        {
            MembershipType = LaboratoryMembershipType.Member,
            NewMemberEmailAddress = "fake@fake.net"
        };

        var response = await client.PostAsJsonAsync("api/invitations", request);
        response.EnsureSuccessStatusCode();
    }

    [Theory]
    [InlineData(LaboratoryMembershipType.Member)]
    [InlineData(LaboratoryMembershipType.Analyst)]
    public async Task InviteMember_Fails_WhenCurrentUserIsNotAdminOrManager(LaboratoryMembershipType membershipType)
    {
        AddTestAuthorization(membershipType);

        var client = CreateClient();

        var request = new InviteLabMemberRequest
        {
            MembershipType = LaboratoryMembershipType.Member,
            NewMemberEmailAddress = "fake@fake.net"
        };

        var response = await client.PostAsJsonAsync("api/invitations", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
