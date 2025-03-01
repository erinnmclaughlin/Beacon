using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.SampleGroups;

namespace Beacon.API.IntegrationTests.Endpoints.Projects.SampleGroups;

[Trait("Feature", "Sample Management")]
public sealed class GetSampleGroupsTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact(DisplayName = "[016] Get project sample groups returns sample groups associated with the specified project")]
    public async Task GetProjectSampleGroups_ReturnsExpectedResults()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetSampleGroupsByProjectIdRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var sampleGroups = await DeserializeAsync<SampleGroupDto[]>(response);
        Assert.NotNull(sampleGroups);
        Assert.Single(sampleGroups);
        Assert.Equal("My Sample Group", sampleGroups[0].SampleName);
    }

    protected override IEnumerable<object> EnumerateTestData() => base.EnumerateTestData().Concat([
        new SampleGroup
        {
            Id = Guid.NewGuid(),
            ProjectId = ProjectId,
            SampleName = "My Sample Group",
            LaboratoryId = TestData.Lab.Id
        },
        new Project
        {
            Id = Guid.NewGuid(),
            CreatedById = TestData.AdminUser.Id,
            CustomerName = "Customer",
            LaboratoryId = TestData.Lab.Id,
            ProjectCode = new ProjectCode("IDK", "202301", 1),
            SampleGroups =
            [
                new SampleGroup
                {
                    Id = Guid.NewGuid(),
                    SampleName = "Other sample group",
                    LaboratoryId = TestData.Lab.Id
                }
            ]
        }
    ]);
}
