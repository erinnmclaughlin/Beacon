using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class GetProjectTypeFrequencyTests(TestFixture fixture)  : TestBase(fixture)
{
    [Fact]
    public async Task GetProjectTypeFrequencyGroupsByMonth()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetProjectTypeFrequencyRequest { StartDate = new DateTime(2023, 1, 1) });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await DeserializeAsync<GetProjectTypeFrequencyRequest.Series[]>(response);
        Assert.NotNull(data);

        var app1Data = data.Single(x => x.ProjectType == "Application 1");
        Assert.Equal(1, app1Data.ProjectCountByDate[new DateOnly(2023, 1, 1)]);
        Assert.Equal(0, app1Data.ProjectCountByDate[new DateOnly(2023, 2, 1)]);

        var app2Data = data.Single(x => x.ProjectType == "Application 2");
        Assert.Equal(0, app2Data.ProjectCountByDate[new DateOnly(2023, 1, 1)]);
        Assert.Equal(2, app2Data.ProjectCountByDate[new DateOnly(2023, 2, 1)]);
    }

    protected override IEnumerable<object> EnumerateTestData()
    {
        foreach (var item in base.EnumerateTestData())
            yield return item;

        yield return new ProjectApplication
        {
            Name = "Application 1",
            LaboratoryId = TestData.Lab.Id,
            TaggedProjects = [
                new ProjectApplicationTag
                {
                    LaboratoryId = TestData.Lab.Id,
                    Project = new Project
                    {
                        Id = Guid.NewGuid(),
                        CustomerName = "Test",
                        ProjectCode = ProjectCode.FromString("TST-202301-001")!,
                        CreatedById = TestData.AdminUser.Id,
                        CreatedOn = new DateTime(2023, 1, 12),
                        LaboratoryId = TestData.Lab.Id
                    }
                }
            ]
        };

        yield return new ProjectApplication
        {
            Name = "Application 2",
            LaboratoryId = TestData.Lab.Id,
            TaggedProjects = [
                new ProjectApplicationTag
                {
                    LaboratoryId = TestData.Lab.Id,
                    Project = new Project
                    {
                        Id = Guid.NewGuid(),
                        CustomerName = "Test",
                        ProjectCode = ProjectCode.FromString("TST-202302-001")!,
                        CreatedById = TestData.AdminUser.Id,
                        CreatedOn = new DateTime(2023, 2, 1),
                        LaboratoryId = TestData.Lab.Id
                    }
                },
                new ProjectApplicationTag
                {
                    LaboratoryId = TestData.Lab.Id,
                    Project = new Project
                    {
                        Id = Guid.NewGuid(),
                        CustomerName = "Test",
                        ProjectCode = ProjectCode.FromString("TST-202302-002")!,
                        CreatedById = TestData.AdminUser.Id,
                        CreatedOn = new DateTime(2023, 2, 28),
                        LaboratoryId = TestData.Lab.Id
                    }
                }
            ]
        };
    }
}
