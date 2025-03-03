using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.SampleGroups;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "[Feature] Project Management")]
public sealed class ProjectManagementSampleGroups(TestFixture fixture) : IntegrationTestBase(fixture)
{
    private static Project DefaultProject => CreateProject(
        id: new Guid("a2871dc3-8746-45ad-bfd8-87e503d397cd"), 
        customerName: "Default Project",
        projectCode: "DFT-202001-001"
    );

    private static SampleGroup DefaultSampleGroup => new()
    {
        Id = new Guid("4fdcebcf-22fa-420a-806c-1f904ce4618a"),
        LaboratoryId = TestData.Lab.Id,
        SampleName = "Default Sample Group"
    };

    protected override IEnumerable<object> EnumerateReseedData()
    {
        var project = DefaultProject;
        project.SampleGroups.Add(DefaultSampleGroup);
        yield return project;
    }

    [Fact(DisplayName = "[016] Create project sample group succeeds when request is valid")]
    public async Task CreateSampleGroup_SucceedsWhenRequestIsValid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new CreateSampleGroupRequest
        {
            ProjectId = DefaultProject.Id,
            SampleName = "My Sample Group"
        });

        response.EnsureSuccessStatusCode();

        Assert.NotNull(await GetSampleGroupByName("My Sample Group"));
    }

    [Fact(DisplayName = "[016] Create project sample group fails when request is invalid")]
    public async Task CreateSampleGroup_FailsWhenRequestIsInvalid()
    {
        await LogInToDefaultLab(TestData.AdminUser);

        var response = await SendAsync(new CreateSampleGroupRequest
        {
            ProjectId = DefaultProject.Id,
            SampleName = ""
        });
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Null(await GetSampleGroupByName(""));
    }

    [Fact(DisplayName = "[016] Create project sample group fails when user is not authorized")]
    public async Task CreateSampleGroup_FailsWhenUserIsUnauthorized()
    {
        await LogInToDefaultLab(TestData.MemberUser);

        var response = await SendAsync(new CreateSampleGroupRequest
        {
            ProjectId = DefaultProject.Id,
            SampleName = "Should Not Succeed"
        });
        
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(await GetSampleGroupByName("Should Not Succeed"));
    }    
    
    [Fact(DisplayName = "[016] Get project sample groups returns sample groups associated with the specified project")]
    public async Task GetProjectSampleGroups_ReturnsExpectedResults()
    {
        // Create a different project that has a sample group (we'll use this to verify that it gets filtered out):
        var otherSampleGroup = new SampleGroup { Id = Guid.NewGuid(), LaboratoryId = TestData.Lab.Id, SampleName = "Other Sample Group" };
        await AddDataAsync(CreateProject("Other Project", "OTH-202001-001", otherSampleGroup));
        
        // Log in as a user that has permission to get project sample groups:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to get sample groups for the default project:
        var response = await SendAsync(new GetSampleGroupsByProjectIdRequest { ProjectId = DefaultProject.Id });

        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();
        
        // Verify that the response contains the expected sample groups:
        var sampleGroups = await response.Content.ReadFromJsonAsync<SampleGroupDto[]>(AbortTest);
        Assert.NotNull(sampleGroups);
        Assert.Contains(sampleGroups, s => s.Id == DefaultSampleGroup.Id);
        Assert.DoesNotContain(sampleGroups, c => c.Id == otherSampleGroup.Id);
    }
    
    private static Project CreateProject(string customerName, string projectCode, params SampleGroup[] sampleGroups) 
        => CreateProject(Guid.NewGuid(), customerName, projectCode, sampleGroups);
    
    private static Project CreateProject(Guid id, string customerName, string projectCode, params SampleGroup[] sampleGroups) => new()
    {
        Id = id,
        CustomerName = customerName,
        ProjectCode = ProjectCode.FromString(projectCode)!,
        ProjectStatus = ProjectStatus.Active,
        CreatedById = TestData.AdminUser.Id,
        LaboratoryId = TestData.Lab.Id,
        SampleGroups = sampleGroups.ToList()
    };
    
    private Task<SampleGroup?> GetSampleGroupByName(string name) => DbContext.SampleGroups
        .IgnoreQueryFilters()
        .Where(x => x.LaboratoryId == TestData.Lab.Id && x.SampleName == name)
        .AsNoTracking()
        .FirstOrDefaultAsync();
}