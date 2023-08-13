using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class GetProjectTypeFrequencyTests : TestBase
{
    public GetProjectTypeFrequencyTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetProjectTypeFrequencyGroupsByMonth()
    {
        RunAsAdmin();

        var request = new GetProjectTypeFrequencyRequest
        {
            StartDate = new DateTime(2023, 1, 1)
        };

        var response = await SendAsync(request);
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

    protected override void AddTestData(BeaconDbContext db)
    {
        var app1 = new ProjectApplication
        {
            Name = "Application 1",
            LaboratoryId = TestData.Lab.Id
        };

        var app2 = new ProjectApplication
        {
            Name = "Application 2",
            LaboratoryId = TestData.Lab.Id
        };

        db.ProjectApplications.AddRange(new[] { app1, app2 });

        db.Projects.AddRange(new []
        {
            new Project
            {
                Id = Guid.NewGuid(),
                CustomerName = "Test",
                ProjectCode = ProjectCode.FromString("TST-202301-001")!,
                CreatedById = TestData.AdminUser.Id,
                CreatedOn = new DateTime(2023, 1, 12),
                LaboratoryId = TestData.Lab.Id,
                TaggedApplications = new()
                {
                    new(){ ApplicationId = app1.Id, LaboratoryId = TestData.Lab.Id }
                }
            },

            new Project
            {
                Id = Guid.NewGuid(),
                CustomerName = "Test",
                ProjectCode = ProjectCode.FromString("TST-202302-001")!,
                CreatedById = TestData.AdminUser.Id,
                CreatedOn = new DateTime(2023, 2, 1),
                LaboratoryId = TestData.Lab.Id,
                TaggedApplications = new()
                {
                    new(){ ApplicationId = app2.Id, LaboratoryId = TestData.Lab.Id }
                }
            },

            new Project
            {
                Id = Guid.NewGuid(),
                CustomerName = "Test",
                ProjectCode = ProjectCode.FromString("TST-202302-002")!,
                CreatedById = TestData.AdminUser.Id,
                CreatedOn = new DateTime(2023, 2, 28),
                LaboratoryId = TestData.Lab.Id,
                TaggedApplications = new()
                {
                    new(){ ApplicationId = app2.Id, LaboratoryId = TestData.Lab.Id }
                }
            },
        });

        base.AddTestData(db);
    }
}
