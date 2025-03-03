using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class GetProjectTypeFrequencyTests(TestFixture fixture)  : IntegrationTestBase(fixture)
{
    [Fact]
    public async Task GetProjectTypeFrequencyGroupsByMonth()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new GetProjectTypeFrequencyRequest { StartDate = new DateTime(2023, 1, 1) });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await response.Content.ReadFromJsonAsync<GetProjectTypeFrequencyRequest.Series[]>(AbortTest);
        Assert.NotNull(data);

        var app1Data = data.Single(x => x.ProjectType == "Application 1");
        Assert.Equal(1, app1Data.ProjectCountByDate[new DateOnly(2023, 1, 1)]);
        Assert.Equal(0, app1Data.ProjectCountByDate[new DateOnly(2023, 2, 1)]);

        var app2Data = data.Single(x => x.ProjectType == "Application 2");
        Assert.Equal(0, app2Data.ProjectCountByDate[new DateOnly(2023, 1, 1)]);
        Assert.Equal(2, app2Data.ProjectCountByDate[new DateOnly(2023, 2, 1)]);
    }

    protected override IEnumerable<object> EnumerateReseedData()
    {
        foreach (var item in base.EnumerateReseedData())
            yield return item;

        var app1 = new ProjectApplication
        {
            Name = "Application 1",
            LaboratoryId = TestData.Lab.Id
        };
        
        yield return app1;

        var project1 = new Project
        {
            Id = Guid.NewGuid(),
            CustomerName = "Test",
            ProjectCode = ProjectCode.FromString("TST-202301-001")!,
            CreatedById = TestData.AdminUser.Id,
            CreatedOn = new DateTime(2023, 1, 12),
            LaboratoryId = TestData.Lab.Id
        };
        
        yield return project1;
        
        yield return new ProjectApplicationTag
        {
            ApplicationId = app1.Id,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = project1.Id
        };
        
        var app2 =  new ProjectApplication
        {
            Name = "Application 2",
            LaboratoryId = TestData.Lab.Id
        };

        yield return app2;

        var project2 = new Project
        {
            Id = Guid.NewGuid(),
            CustomerName = "Test",
            ProjectCode = ProjectCode.FromString("TST-202302-001")!,
            CreatedById = TestData.AdminUser.Id,
            CreatedOn = new DateTime(2023, 2, 1),
            LaboratoryId = TestData.Lab.Id
        };
        
        yield return project2;

        yield return new ProjectApplicationTag
        {
            ApplicationId = app2.Id,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = project2.Id
        };
        
        var project3 = new Project
        {
            Id = Guid.NewGuid(),
            CustomerName = "Test",
            ProjectCode = ProjectCode.FromString("TST-202302-002")!,
            CreatedById = TestData.AdminUser.Id,
            CreatedOn = new DateTime(2023, 2, 28),
            LaboratoryId = TestData.Lab.Id
        };

        yield return project3;
        
        yield return new ProjectApplicationTag
        {
            ApplicationId = app2.Id,
            LaboratoryId = TestData.Lab.Id,
            ProjectId = project3.Id
        };
    }
}
