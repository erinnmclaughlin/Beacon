using Beacon.Common.Models;

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
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync($"api/projects/{ProjectCode}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await DeserializeAsync<ProjectDto>(response);
        Assert.NotNull(project);
        Assert.Equal(ProjectId, project.Id);
        Assert.Equal(ProjectCode.ToString(), project.ProjectCode);
    }


    [Fact(DisplayName = "[193] Get project endpoint returns project details when project ID is valid")]
    public async Task GetProject_Succeeds_WhenProjectIdIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync($"api/projects/{ProjectId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await DeserializeAsync<ProjectDto>(response);
        Assert.NotNull(project);
        Assert.Equal(ProjectId, project.Id);
        Assert.Equal(ProjectCode.ToString(), project.ProjectCode);
    }

    [Fact(DisplayName = "[193] Get project endpoint returns 400 when project code is in invalid format")]
    public async Task GetProject_ReturnsBadRequest_WhenProjectCodeIsInInvalidFormat()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync("api/projects/invalid");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact(DisplayName = "[193] Get project endpoint returns 403 when user is not authorized")]
    public async Task GetProject_ReturnsForbidden_WhenUserIsNotAuthorized()
    {
        SetCurrentUser(TestData.NonMemberUser.Id);

        var response = await GetAsync($"api/projects/{ProjectId}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
