using Beacon.Common.Memberships;

namespace Beacon.IntegrationTests.EndpointTests.Projects;

[Collection("ProjectTests")]
public class GetProjectTests : EndpointTestBase
{
    public GetProjectTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);
    }

    [Fact]
    public async Task GetProject_ReturnsBadRequest_IfProjectCodeIsInvalid()
    {
        var response = await CreateClient().AddLabHeader().GetAsync($"api/projects/invalid");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProjectById_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        var response = await CreateClient().AddLabHeader().GetAsync($"api/projects/IDK-123");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
