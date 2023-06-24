namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

[Collection("LaboratoryTests")]
public class GetLaboratoryByIdTests : EndpointTestBase
{
    public GetLaboratoryByIdTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetLaboratoryById_SucceedsWhenUserIsMember()
    {
        AddTestAuthorization(Common.Memberships.LaboratoryMembershipType.Member);

        var response = await CreateClient().GetAsync($"api/laboratories/{TestData.DefaultLaboratory.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
