using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class GetProjectDetailsTests : ProjectTestBase
{
    public GetProjectDetailsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[193] Get project endpoint returns project details when project code is valid")]
    public async Task GetProject_Succeeds_WhenProjectCodeIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetProjectByProjectCodeRequest { ProjectCode = ProjectCode });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await DeserializeAsync<ProjectDto>(response);
        Assert.NotNull(project);
        Assert.Equal(ProjectId, project.Id);
        Assert.Equal(ProjectCode.ToString(), project.ProjectCode);
    }


    [Fact(DisplayName = "[193] Get project endpoint returns project details when project ID is valid")]
    public async Task GetProject_Succeeds_WhenProjectIdIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetProjectByIdRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await DeserializeAsync<ProjectDto>(response);
        Assert.NotNull(project);
        Assert.Equal(ProjectId, project.Id);
        Assert.Equal(ProjectCode.ToString(), project.ProjectCode);
    }

    [Fact(DisplayName = "[193] Get project endpoint returns 403 when user is not authorized")]
    public async Task GetProject_ReturnsForbidden_WhenUserIsNotAuthorized()
    {
        RunAsNonMember();

        var response = await SendAsync(new GetProjectByIdRequest { ProjectId = ProjectId });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
