using Beacon.Common.Memberships;

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

        var uri = $"api/memberships?laboratoryId={TestData.DefaultLaboratory.Id}";
        var members = await CreateClient().GetFromJsonAsync<LaboratoryMemberDto[]>(uri, JsonSerializerOptions);

        members.Should().ContainSingle().Which.Id.Should().Be(TestData.DefaultUser.Id);

    }
}
