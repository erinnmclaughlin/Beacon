using Beacon.API.IntegrationTests.Collections;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public sealed class CreateProjectTests : ProjectTestBase
{
    private static CreateProjectRequest SomeValidRequest { get; } = new()
    {
        CustomerCode = "ABC",
        CustomerName = "ABC Company"
    };

    public static CreateProjectRequest SomeInvalidRequest { get; } = new()
    {
        CustomerCode = "ABCD",
        CustomerName = "ABC Company"
    };

    public CreateProjectTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[004] Create project suceeds when request is valid")]
    public async Task CreateProject_Succeeds_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/projects", SomeValidRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdProject = ExecuteDbContext(db => db.Projects.Single(x => x.CustomerName == SomeValidRequest.CustomerName));
        Assert.Equal("ABC-001", createdProject.ProjectCode.ToString());
        Assert.Equal(TestData.AdminUser.Id, createdProject.CreatedById);
        Assert.Equal(TestData.Lab.Id, createdProject.LaboratoryId);
        Assert.Equal(ProjectStatus.Active, createdProject.ProjectStatus);
    }

    [Fact(DisplayName = "[004] Create project fails when request is invalid")]
    public async Task CreateProject_Fails_WhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/projects", SomeInvalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdProject = ExecuteDbContext(db => db.Projects.SingleOrDefault(x => x.CustomerName == SomeInvalidRequest.CustomerName));
        Assert.Null(createdProject);
    }

    [Fact(DisplayName = "[004] Create project fails when user is not authorized")]
    public async Task CreateProject_Fails_WhenUserIsNotAuthorized()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var response = await PostAsync("api/projects", SomeValidRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdProject = ExecuteDbContext(db => db.Projects.SingleOrDefault(x => x.CustomerName == SomeValidRequest.CustomerName));
        Assert.Null(createdProject);
    }
}
