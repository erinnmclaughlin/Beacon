using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class CancelProjectTests : ProjectTestBase
{
    public CancelProjectTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[005] Cancel project succeeds when request is valid")]
    public async Task CancelProject_SucceedsWhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/projects/cancel", new CancelProjectRequest
        {
            ProjectId = ProjectId
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var project = ExecuteDbContext(db => db.Projects.Single(x => x.Id == ProjectId));
        Assert.Equal(ProjectStatus.Canceled, project.ProjectStatus);
    }

    [Fact(DisplayName = "[005] Cancel project fails when user is unauthorized")]
    public async Task CancelProject_FailsWhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var response = await PostAsync("api/projects/cancel", new CancelProjectRequest
        {
            ProjectId = ProjectId
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var project = ExecuteDbContext(db => db.Projects.Single(x => x.Id == ProjectId));
        Assert.Equal(ProjectStatus.Active, project.ProjectStatus);
    }
}
