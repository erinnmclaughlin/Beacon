using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class CancelProjectTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact(DisplayName = "[005] Cancel project succeeds when request is valid")]
    public async Task CancelProject_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CancelProjectRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var projectStatus = await GetProjectStatusAsync();
        Assert.Equal(ProjectStatus.Canceled, projectStatus);
    }

    [Fact(DisplayName = "[005] Cancel project fails when user is unauthorized")]
    public async Task CancelProject_FailsWhenRequestIsInvalid()
    {
        RunAsMember();

        var response = await SendAsync(new CancelProjectRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var projectStatus = await GetProjectStatusAsync();
        Assert.Equal(ProjectStatus.Active, projectStatus);
    }
}
