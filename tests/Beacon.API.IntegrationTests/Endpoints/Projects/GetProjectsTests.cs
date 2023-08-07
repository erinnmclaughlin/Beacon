using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class GetProjectsTests : ProjectTestBase
{
    public GetProjectsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[193] Get lab projects endpoint returns list of lab projects when current user is authorized")]
    public async Task GetProjects_ReturnsOnlyProjectsAssociatedWithCurrentLab()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetProjectsRequest());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var projects = await DeserializeAsync<PagedList<ProjectDto>>(response);

        Assert.NotNull(projects);
        var project = Assert.Single(projects.Items);
        Assert.Equal(ProjectId, project.Id);        
    }

    [Fact(DisplayName = "[193] Get lab projects endpoint returns 403 when user is not authorized")]
    public async Task GetProjects_FailsWhenUserIsUnauthorized()
    {
        RunAsNonMember();

        var response = await SendAsync(new GetProjectsRequest());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "[275] Pagination is applied to project search")]
    public async Task PaginationIsApplied()
    {
        RunAsAdmin();

        var request = new GetProjectsRequest { PageSize = 1 };
        var response = await SendAsync(new GetProjectsRequest());
        var projects = await DeserializeAsync<PagedList<ProjectDto>>(response);
        Assert.NotNull(projects);
        Assert.Equal(1, projects.TotalCount);
        Assert.Single(projects.Items);
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
            ProjectCode = new ProjectCode("AAA", "202301", 1),
            ProjectStatus = ProjectStatus.Active
        };

        db.Laboratories.Add(otherLab);
        db.Projects.Add(project);

        base.AddTestData(db);
    }
}
