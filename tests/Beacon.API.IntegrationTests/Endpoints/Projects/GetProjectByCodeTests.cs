using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class GetProjectByCodeTests : ProjectTestBase
{
    public GetProjectByCodeTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Get project succeeds when request is valid")]
    public async Task GetProject_Succeeds_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync($"api/projects/{Project.ProjectCode}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var project = await DeserializeAsync<ProjectDto>(response);
        Assert.NotNull(project);
        Assert.Equal(Project.Id, project.Id);
        Assert.Equal(Project.ProjectCode.ToString(), project.ProjectCode);
    }

    [Fact(DisplayName = "Get project returns bad request when project code is in invalid format")]
    public async Task GetProject_ReturnsBadRequest_WhenProjectCodeIsInInvalidFormat()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync("api/projects/invalid");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
