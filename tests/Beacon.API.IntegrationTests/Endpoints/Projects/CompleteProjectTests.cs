using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class CompleteProjectTests : ProjectTestBase
{
    public CompleteProjectTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Complete project succeeds when request is valid")]
    public async Task CompleteProject_SucceedsWhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/projects/complete", new CompleteProjectRequest
        {
            ProjectId = ProjectId
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var project = ExecuteDbContext(db => db.Projects.Single(x => x.Id == ProjectId));
        Assert.Equal(ProjectStatus.Completed, project.ProjectStatus);
    }

    [Fact(DisplayName = "Complete project fails when request is invalid")]
    public async Task CompleteProject_FailsWhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var response = await PostAsync("api/projects/complete", new CompleteProjectRequest
        {
            ProjectId = ProjectId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var project = ExecuteDbContext(db => db.Projects.Single(x => x.Id == ProjectId));
        Assert.Equal(ProjectStatus.Active, project.ProjectStatus);
    }
}
