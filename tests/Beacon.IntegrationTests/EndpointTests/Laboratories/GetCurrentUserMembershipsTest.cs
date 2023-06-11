using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Enums;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class GetCurrentUserMembershipsTest : EndpointTestBase
{
    private readonly HttpClient _httpClient;
    public GetCurrentUserMembershipsTest(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization();
        _httpClient = CreateClient();
    }

    [Fact]
    public async Task GetCurrentUserMemberships_ShouldReturnExpectedMemberships()
    {
        var memberships = await _httpClient
            .GetFromJsonAsync<List<LaboratoryMembershipDto>>("api/users/me/memberships", JsonSerializerOptions);

        var membership = memberships.Should().ContainSingle().Which;
        membership.Laboratory.Id.Should().Be(TestData.DefaultLaboratory.Id);
        membership.MembershipType.Should().Be(LaboratoryMembershipType.Admin);
        membership.Member.Id.Should().Be(TestData.DefaultUser.Id);
    }
}
