using Beacon.Common.Models;

namespace Beacon.IntegrationTests.EndpointTests.Members;

[Collection("MembershipTests")]
public sealed class GetLaboratoryMembersTests : EndpointTestBase
{
    public GetLaboratoryMembersTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetMembers_ReturnsMembers()
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);

        var members = await CreateClient()
            .AddLabHeader()
            .GetFromJsonAsync<LaboratoryMemberDto[]>("api/memberships", JsonSerializerOptions);

        members.Should().ContainSingle().Which.Id.Should().Be(TestData.DefaultUser.Id);

    }
}
