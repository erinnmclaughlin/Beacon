using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.API.Persistence.Extensions;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "Project Management")]
public sealed class ProjectManagementApiTests(TestFixture fixture) : IntegrationTestBase(fixture)
{
    private static Project DefaultProject => new()
    {
        Id = new Guid("a2871dc3-8746-45ad-bfd8-87e503d397cd"),
        CustomerName = "ABC Company",
        ProjectCode = new ProjectCode("ABC", "200101", 1),
        ProjectStatus = ProjectStatus.Active,
        CreatedById = TestData.AdminUser.Id,
        CreatedOn = new DateTime(2020, 01, 01),
        LaboratoryId = TestData.Lab.Id,
        LeadAnalystId = null
    };
    
    protected override IEnumerable<object> EnumerateCustomSeedData()
    {
        yield return DefaultProject;
    }
    
    [Theory(DisplayName = "[004] Authorized users can create laboratory projects")]
    [InlineData(LaboratoryMembershipType.Admin)]
    [InlineData(LaboratoryMembershipType.Manager)]
    [InlineData(LaboratoryMembershipType.Analyst)]
    public async Task CreateProject_Succeeds_WhenRequestIsValid(LaboratoryMembershipType membershipType)
    {
        // Log in as a user of the specified type:
        var user = GetDefaultUserForMembershipType(membershipType);
        await LogInToDefaultLab(user);

        // Attempt to create a new project:
        var response = await HttpClient.SendAsync(new CreateProjectRequest
        {
            CustomerCode = "XYZ",
            CustomerName = "XYZ Company"
        });
        
        // Verify that this succeeds:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify that the persisted information matches the request:
        var code = await response.Content.ReadFromJsonAsync<ProjectCode>();
        Assert.NotNull(code);
        var createdProject = await DbContext.Projects.IgnoreQueryFilters().WithCode(code).SingleAsync();
        Assert.NotNull(createdProject);
        Assert.Equal(user.Id, createdProject.CreatedById);
        Assert.Equal(TestData.Lab.Id, createdProject.LaboratoryId);
        Assert.Equal(ProjectStatus.Active, createdProject.ProjectStatus);
    }
    
    [Fact(DisplayName = "[004] Cannot create a laboratory project with an invalid customer code")]
    public async Task CreateProject_Fails_WhenRequestIsInvalid()
    {
        // Login as a user that has permission to create projects:
        await LogInToDefaultLab(TestData.AdminUser);

        // Attempt to create a project with an invalid customer code:
        var response = await HttpClient.SendAsync(new CreateProjectRequest
        {
            CustomerCode = "WXYZ", // invalid code (must be 3 characters)
            CustomerName = "XYZ Company"
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact(DisplayName = "[004] Cannot create a laboratory project with an unauthorized lead analyst")]
    public async Task CreateProject_Fails_WhenLeadAnalystIsNotAuthorized()
    {
        // Login as a user that has permission to create projects:
        await LogInToDefaultLab(TestData.AdminUser);

        // Attempt to create a project with an invalid lead analyst:
        var response = await HttpClient.SendAsync(new CreateProjectRequest
        {
            CustomerCode = "XYZ",
            CustomerName = "XYZ Company",
            LeadAnalystId = TestData.MemberUser.Id // can't be assigned as analyst
        });

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact(DisplayName = "[004] Unauthorized users cannot create laboratory projects")]
    public async Task CreateProject_Fails_WhenUserIsNotAuthorized()
    {
        // Login as a user that does NOT have permission to create projects:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to create a new project:
        var response = await HttpClient.SendAsync(new CreateProjectRequest
        {
            CustomerCode = "XYZ",
            CustomerName = "XYZ Company"
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact(DisplayName = "[005] Authorized users can complete laboratory projects")]
    public async Task CompleteProject_SucceedsWhenRequestIsValid()
    {
        // Log in as a user that has permission to complete projects:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to complete the default test project:
        var response = await HttpClient.SendAsync(new CompleteProjectRequest { ProjectId = DefaultProject.Id });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the project status has been updated:
        Assert.Equal(ProjectStatus.Completed, await GetDefaultProjectStatus());

        // Reset the database:
        ShouldResetDatabase = true;
    }

    [Fact(DisplayName = "[005] Unauthorized users cannot complete laboratory projects")]
    public async Task CompleteProject_FailsWhenRequestIsInvalid()
    {
        // Log in as a user that does NOT have permission to complete projects:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to complete the default test project:
        var response = await HttpClient.SendAsync(new CompleteProjectRequest { ProjectId = DefaultProject.Id });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // Verify that the project status has NOT been updated:
        Assert.Equal(ProjectStatus.Active, await GetDefaultProjectStatus());
    }
    
    [Fact(DisplayName = "[005] Authorized users can cancel laboratory projects")]
    public async Task CancelProject_SucceedsWhenRequestIsValid()
    {
        // Log in as a user that has permission to cancel  projects:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to cancel the default test project:
        var response = await HttpClient.SendAsync(new CancelProjectRequest { ProjectId = DefaultProject.Id });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the project status has been updated:
        Assert.Equal(ProjectStatus.Canceled, await GetDefaultProjectStatus());

        // Reset the database:
        ShouldResetDatabase = true;
    }

    [Fact(DisplayName = "[005] Unauthorized users cannot cancel laboratory projects")]
    public async Task CancelProject_FailsWhenRequestIsInvalid()
    {
        // Log in as a user that does NOT have permission to cancel projects:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to cancel the default test project:
        var response = await HttpClient.SendAsync(new CancelProjectRequest { ProjectId = DefaultProject.Id });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        // Verify that the project status has NOT been updated:
        Assert.Equal(ProjectStatus.Active, await GetDefaultProjectStatus());
    }
    
    [Fact(DisplayName = "[014] Authorized users can assign a lead analyst to an existing project")]
    public async Task AssigningLeadAnalyst_ShouldSucceed_WhenRequestIsValid()
    {
        // Log in as a user that has permission to assign lead analysts:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to assign a valid lead analyst:
        var response = await HttpClient.SendAsync(new UpdateLeadAnalystRequest
        {
            ProjectId = DefaultProject.Id,
            AnalystId = TestData.AnalystUser.Id
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();
        
        // Verify that the lead analyst has been updated:
        Assert.Equal(TestData.AnalystUser.Id, await GetDefaultProjectAnalystId());
        
        // Reset the database:
        ShouldResetDatabase = true;
    }

    [Fact(DisplayName = "[014] Cannot update lead analyst to an unauthorized user")]
    public async Task AssigningLeadAnalyst_ShouldFail_WhenAnalystIsNotValid()
    {
        // Log in as a user that has permission to assign lead analysts:
        await LogInToDefaultLab(TestData.AdminUser);

        // Attempt to assign a lead analyst that is not in a valid role:
        var response = await HttpClient.SendAsync(new UpdateLeadAnalystRequest
        {
            ProjectId = DefaultProject.Id,
            AnalystId = TestData.MemberUser.Id
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        
        // Verify that the lead analyst has NOT been updated:
        Assert.Null(await GetDefaultProjectAnalystId());
    }
    
    [Fact(DisplayName = "[014] Authorized users can un-assign the lead analyst on an existing project")]
    public async Task UnassigningLeadAnalyst_ShouldSucceed_WhenRequestIsValid()
    {
        // Update the default project to have a lead analyst:
        await DbContext.Projects.IgnoreQueryFilters().Where(x => x.Id == DefaultProject.Id)
            .ExecuteUpdateAsync(x => x.SetProperty(p => p.LeadAnalystId, TestData.AnalystUser.Id));
        
        // Sanity check to make sure the current analyst was set:
        Assert.Equal(TestData.AnalystUser.Id, await GetDefaultProjectAnalystId());
        
        // Log in as a user that has permission to un-assign lead analysts:
        await LogInToDefaultLab(TestData.AdminUser);

        // Attempt to un-assign the lead analyst:
        var response = await HttpClient.SendAsync(new UpdateLeadAnalystRequest
        {
            ProjectId = DefaultProject.Id,
            AnalystId = null
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();
        
        // Verify that the lead analyst has been updated:
        Assert.Null(await GetDefaultProjectAnalystId());
        
        // We don't need to reset the database because the end state is equivalent to the start state:
        // ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[193] Authorized users can get project details by project code")]
    public async Task GetProjectByCode_Succeeds_WhenProjectCodeIsValid()
    {
        // Log in as a user that can view lab projects:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to get the default project details by code:
        var response = await HttpClient.SendAsync(new GetProjectByProjectCodeRequest { ProjectCode = DefaultProject.ProjectCode });
       
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the project details are correct:
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(project);
        Assert.Equal(DefaultProject.Id, project.Id);
        Assert.Equal(DefaultProject.ProjectCode.ToString(), project.ProjectCode);
        Assert.Equal(DefaultProject.ProjectStatus, project.ProjectStatus);
        Assert.Equal(DefaultProject.CustomerName, project.CustomerName);
    }
    
    [Fact(DisplayName = "[193] Unauthorized users cannot get project details by project code")]
    public async Task GetProjectByCode_ReturnsForbidden_WhenUserIsNotAuthorized()
    {
        // Log in as a user that does NOT have permission to view lab projects:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to get the default project details by code:
        var response = await HttpClient.SendAsync(new GetProjectByProjectCodeRequest { ProjectCode = DefaultProject.ProjectCode });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact(DisplayName = "[193] Authorized users can get project details by project id")]
    public async Task GetProjectById_Succeeds_WhenProjectIdIsValid()
    {
        // Log in as a user that can view lab projects:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to get the default project details by ID:
        var response = await HttpClient.SendAsync(new GetProjectByIdRequest { ProjectId = DefaultProject.Id });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the project details are correct:
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(project);
        Assert.Equal(DefaultProject.Id, project.Id);
        Assert.Equal(DefaultProject.ProjectCode.ToString(), project.ProjectCode);
        Assert.Equal(DefaultProject.ProjectStatus, project.ProjectStatus);
        Assert.Equal(DefaultProject.CustomerName, project.CustomerName);
    }

    [Fact(DisplayName = "[193] Unauthorized users cannot get project details by project id")]
    public async Task GetProjectById_ReturnsForbidden_WhenUserIsNotAuthorized()
    {
        // Log in as a user that does NOT have permission to view lab projects:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to get the default project details by code:
        var response = await HttpClient.SendAsync(new GetProjectByIdRequest { ProjectId = DefaultProject.Id });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "[193] Lab projects are filtered by current laboratory")]
    public async Task GetProjects_ReturnsOnlyProjectsAssociatedWithCurrentLab()
    {
        // Create another lab with a project and add the admin user as a member:
        var otherLab = new Laboratory { Name = "Some other lab" };
        var otherLabProject = new Project
        {
            Id = Guid.NewGuid(),
            CreatedById = TestData.AdminUser.Id,
            CustomerName = "Some other customer",
            ProjectCode = new ProjectCode("AAA", "202301", 1),
            ProjectStatus = ProjectStatus.Active
        };
        otherLab.AddAdmin(TestData.AdminUser);
        otherLab.Projects.Add(otherLabProject);
        DbContext.Add(otherLab);
        await DbContext.SaveChangesAsync();
        
        // Log in to the default lab:
        await LogInToDefaultLab(TestData.AdminUser);

        // Attempt to get laboratory projects:
        var response = await HttpClient.SendAsync(new GetProjectsRequest());
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the response only contains projects for the default lab:
        var projects = await response.Content.ReadFromJsonAsync<PagedList<ProjectDto>>();
        Assert.NotNull(projects);
        Assert.Contains(projects.Items, p => p.ProjectCode == DefaultProject.ProjectCode.ToString());
        Assert.DoesNotContain(projects.Items, p => p.ProjectCode == otherLab.Projects[0].ProjectCode.ToString());
        
        // Switch to the other lab:
        await SetCurrentLab(otherLab.Id);
        
        // Attempt to get laboratory projects again:
        response = await HttpClient.SendAsync(new GetProjectsRequest());
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();
        
        // Verify that the response only contains projects for the other lab:
        projects = await response.Content.ReadFromJsonAsync<PagedList<ProjectDto>>();
        Assert.NotNull(projects);
        Assert.DoesNotContain(projects.Items, p => p.ProjectCode == DefaultProject.ProjectCode.ToString());
        Assert.Contains(projects.Items, p => p.ProjectCode == otherLab.Projects[0].ProjectCode.ToString());

        // Reset the database:
        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[193] Unauthorized users cannot get lab projects")]
    public async Task GetProjects_FailsWhenUserIsUnauthorized()
    {
        // Log in as a user that does NOT have permission to view lab projects:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to get laboratory projects:
        var response = await HttpClient.SendAsync(new GetProjectsRequest());
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact(DisplayName = "[275] Pagination is applied to project search")]
    public async Task PaginationIsApplied()
    {
        // add an extra project to the default lab
        await AddDataAsync(new Project
        {
            Id = Guid.NewGuid(),
            LaboratoryId = TestData.Lab.Id,
            CustomerName = "Another project",
            ProjectCode = new ProjectCode("OTH", "202301", 1),
            ProjectStatus = ProjectStatus.Active,
            CreatedById = TestData.AdminUser.Id
        });
        
        // Sanity check: make sure lab has multiple projects:
        var projectCount = await DbContext.Projects.IgnoreQueryFilters().CountAsync(x => x.LaboratoryId == TestData.Lab.Id);
        Assert.True(projectCount > 1);
        
        // Log in as a user that can view lab projects:
        await LogInToDefaultLab(TestData.MemberUser);

        var response = await HttpClient.SendAsync(new GetProjectsRequest { PageSize = 1 });
        var projects = await response.Content.ReadFromJsonAsync<PagedList<ProjectDto>>();
        Assert.NotNull(projects);
        Assert.Equal(1, projects.PageSize);
        Assert.Equal(projectCount, projects.TotalCount);
        Assert.Single(projects.Items);
    }
    
    private Task<ProjectStatus> GetDefaultProjectStatus() => DbContext.Projects
        .IgnoreQueryFilters()
        .Where(x => x.Id == DefaultProject.Id)
        .Select(x => x.ProjectStatus)
        .SingleAsync();
    
    private Task<Guid?> GetDefaultProjectAnalystId() => DbContext.Projects
        .IgnoreQueryFilters()
        .Where(x => x.Id == DefaultProject.Id)
        .Select(x => x.LeadAnalystId)
        .SingleAsync();
}
