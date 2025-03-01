using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

[Trait("Feature", "Project Management")]
public sealed class CreateProjectTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    private static CreateProjectRequest SomeValidRequest => new()
    {
        CustomerCode = "ABC",
        CustomerName = "ABC Company"
    };

    private static CreateProjectRequest SomeInvalidRequest => new()
    {
        CustomerCode = "ABCD",
        CustomerName = "ABC Company"
    };

    [Fact(DisplayName = "[004] Create project succeeds when request is valid")]
    public async Task CreateProject_Succeeds_WhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var createdProject = await GetProjectAsync(SomeValidRequest.CustomerName);
        Assert.NotNull(createdProject);
        Assert.Equal($"ABC-{DateTime.Today:yyyyMM}-001", createdProject.ProjectCode.ToString());
        Assert.Equal(TestData.AdminUser.Id, createdProject.CreatedById);
        Assert.Equal(TestData.Lab.Id, createdProject.LaboratoryId);
        Assert.Equal(ProjectStatus.Active, createdProject.ProjectStatus);
    }

    [Fact(DisplayName = "[004] Create project fails when request is invalid")]
    public async Task CreateProject_Fails_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(SomeInvalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var createdProject = await GetProjectAsync(SomeInvalidRequest.CustomerName);
        Assert.Null(createdProject);
    }

    [Fact(DisplayName = "[004] Create project fails when lead analyst is not authorized")]
    public async Task CreateProject_Fails_WhenLeadAnalystIsNotAuthorized()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateProjectRequest
        {
            CustomerCode = SomeValidRequest.CustomerCode,
            CustomerName = SomeValidRequest.CustomerName,
            LeadAnalystId = TestData.MemberUser.Id
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        
        var createdProject = await GetProjectAsync(SomeInvalidRequest.CustomerName);
        Assert.Null(createdProject);
    }

    [Fact(DisplayName = "[004] Create project fails when user is not authorized")]
    public async Task CreateProject_Fails_WhenUserIsNotAuthorized()
    {
        RunAsMember();

        var response = await SendAsync(SomeValidRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var createdProject = await GetProjectAsync(SomeValidRequest.CustomerName);
        Assert.Null(createdProject);
    }

    private async Task<Project?> GetProjectAsync(string customerName) => await ExecuteDbContextAsync(async db =>
    {
        return await db.Projects.AsNoTracking().SingleOrDefaultAsync(x => x.CustomerName == customerName);
    });
}
