using Beacon.Common.Laboratories;

namespace Beacon.IntegrationTests.EndpointTests.Projects;

[Collection("ProjectTests")]
public class GetProjectByIdOrCodeTests : EndpointTestBase
{
    public GetProjectByIdOrCodeTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);
    }

    [Fact]
    public async Task GetProject_ReturnsBadRequest_IfSlugIsNotGuidOrProjectId()
    {
        var response = await CreateClient().GetAsync("api/projects/invalid");
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProjectByCode_ReturnsNotFound_IfCodeIsInvalid()
    {
        var response = await CreateClient().GetAsync("api/projects/ABC-123");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProjectById_ReturnsNotFound_IfIdIsInvalid()
    {
        var response = await CreateClient().GetAsync($"api/projects/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
