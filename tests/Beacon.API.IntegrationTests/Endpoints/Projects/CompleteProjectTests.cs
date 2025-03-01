using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class CompleteProjectTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact(DisplayName = "[005] Complete project succeeds when request is valid")]
    public async Task CompleteProject_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CompleteProjectRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var projectStatus = await GetProjectStatusAsync();
        Assert.Equal(ProjectStatus.Completed, projectStatus);
    }

    [Fact(DisplayName = "[005] Complete project fails when request is invalid")]
    public async Task CompleteProject_FailsWhenRequestIsInvalid()
    {
        RunAsMember();

        var response = await SendAsync(new CompleteProjectRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var projectStatus = await GetProjectStatusAsync();
        Assert.Equal(ProjectStatus.Active, projectStatus);
    }
}
