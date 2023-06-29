using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class GetProjectsTests : ProjectTestBase
{
    public GetProjectsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Get projects returns only projects associated with current lab")]
    public async Task GetProjects_ReturnsOnlyProjectsAssociatedWithCurrentLab()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync("api/projects");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await DeserializeAsync<ProjectDto[]>(response);

        Assert.NotNull(projects);
        Assert.Single(projects);
        Assert.Equal(Project.Id, projects[0].Id);        
    }

    [Fact(DisplayName = "Get projects fails when user is not authorized")]
    public async Task GetProjects_FailsWhenUserIsUnauthorized()
    {
        SetCurrentUser(TestData.NonMemberUser.Id);

        var response = await GetAsync("api/projects");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        var otherLab = new Laboratory { Id = Guid.NewGuid(), Name = "Some other lab" };

        otherLab.AddMember(TestData.AdminUser.Id, LaboratoryMembershipType.Admin);

        var project = new Project
        {
            Id = Guid.NewGuid(),
            CreatedById = TestData.AdminUser.Id,
            CustomerName = "Some customer",
            LaboratoryId = otherLab.Id,
            ProjectCode = new ProjectCode("AAA", 1),
            ProjectStatus = ProjectStatus.Active
        };

        db.Laboratories.Add(otherLab);
        db.Projects.Add(project);

        base.AddTestData(db);
    }
}
