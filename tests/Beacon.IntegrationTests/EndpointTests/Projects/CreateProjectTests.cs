using Beacon.Common.Memberships;
using Beacon.Common.Projects.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Projects;

[Collection("ProjectTests"), CollectionDefinition("ProjectTests", DisableParallelization = true)]
public class CreateProjectTests : EndpointTestBase
{
    public CreateProjectTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateProject_FailsWhenUserIsInUnauthorizedRole()
    {
        AddTestAuthorization(LaboratoryMembershipType.Member);
        var response = await CreateClient().PostAsJsonAsync("api/projects", new CreateProjectRequest
        {
            CustomerCode = "ABC",
            CustomerName = "ABC Company"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateProject_SucceedsWhenRequestIsValid()
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);
        var response = await CreateClient().PostAsJsonAsync("api/projects", new CreateProjectRequest
        {
            CustomerCode = "ABC",
            CustomerName = "ABC Company"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
