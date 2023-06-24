using Beacon.App.Entities;
using Beacon.App.ValueObjects;
using Beacon.Common.Memberships;
using Beacon.Common.Projects.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Projects;

[Collection("ProjectTests")]
public class CancelProjectTests : EndpointTestBase
{
    public CancelProjectTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CancelProject_SucceedsWhenRequestIsValid()
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);
        var projectId = Guid.NewGuid();
        var client = CreateClient(db =>
        {
            db.Projects.Add(new Project
            {
                Id = projectId,
                CustomerName = "Test Customer",
                ProjectCode = new ProjectCode("TST", 1),
                CreatedById = TestData.DefaultUser.Id,
                LaboratoryId = TestData.DefaultLaboratory.Id
            });

            db.SaveChanges();
        });

        var request = new CancelProjectRequest { LaboratoryId = TestData.DefaultLaboratory.Id, ProjectId = projectId };
        var response = await client.PostAsJsonAsync($"api/projects/cancel", request);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
