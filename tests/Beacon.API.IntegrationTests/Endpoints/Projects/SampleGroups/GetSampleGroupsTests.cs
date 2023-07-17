using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.SampleGroups;

[Trait("Feature", "Sample Management")]
public sealed class GetSampleGroupsTests : ProjectTestBase
{
    public GetSampleGroupsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[016] Get project sample groups returns sample groups associated with the specified project")]
    public async Task GetProjectSampleGroups_ReturnsExpectedResults()
    {
        RunAsAdmin();

        var response = await GetAsync($"api/projects/{ProjectId}/sample-groups");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var sampleGroups = await DeserializeAsync<SampleGroupDto[]>(response);
        Assert.NotNull(sampleGroups);
        Assert.Single(sampleGroups);
        Assert.Equal("My Sample Group", sampleGroups[0].SampleName);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        var otherProject = new Project
        {
            Id = Guid.NewGuid(),
            CreatedById = TestData.AdminUser.Id,
            CustomerName = "Customer",
            LaboratoryId = TestData.Lab.Id,
            ProjectCode = new ProjectCode("IDK", 1)
        };

        otherProject.SampleGroups.Add(new SampleGroup
        {
            Id = Guid.NewGuid(),
            SampleName = "Other sample group",
            LaboratoryId = TestData.Lab.Id
        });

        db.Projects.Add(otherProject);
        db.SampleGroups.Add(new SampleGroup
        {
            Id = Guid.NewGuid(),
            ProjectId = ProjectId,
            SampleName = "My Sample Group",
            LaboratoryId = TestData.Lab.Id
        });

        base.AddTestData(db);
    }
}
