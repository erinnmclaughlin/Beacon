using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "API")]
[Trait("Category", "Member Management")]
public class MemberManagementApiTests(TestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact(DisplayName = "[170] Get memberships endpoint returns list of lab members when user is authorized")]
    public async Task GetMemberships_ReturnsExpectedResult_WhenUserIsMember()
    {
        // Log in as someone that has permission to view lab information:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to view the list of memberships:
        var response = await SendAsync(new GetMembershipsRequest());
       
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the response content contains the expected information about the members:
        var memberships = await response.Content.ReadFromJsonAsync<LaboratoryMemberDto[]>(AbortTest);
        Assert.NotNull(memberships);
        Assert.Contains(memberships, m => m.Id == TestData.AdminUser.Id && m.MembershipType == LaboratoryMembershipType.Admin);
        Assert.Contains(memberships, m => m.Id == TestData.ManagerUser.Id && m.MembershipType == LaboratoryMembershipType.Manager);
        Assert.Contains(memberships, m => m.Id == TestData.AnalystUser.Id && m.MembershipType == LaboratoryMembershipType.Analyst);
        Assert.Contains(memberships, m => m.Id == TestData.MemberUser.Id && m.MembershipType == LaboratoryMembershipType.Member);
        Assert.DoesNotContain(memberships, m => m.Id == TestData.NonMemberUser.Id);
    }
    
    [Fact(DisplayName = "[170] Get memberships endpoint returns 403 when user is not authorized")]
    public async Task GetMemberships_FailsWhenUserIsNotAMember()
    {
        // Log in as someone that does NOT have permission to view lab information:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to view the list of memberships:
        var response = await SendAsync(new GetMembershipsRequest());
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact(DisplayName = "[170] Update membership type succeeds when user is authorized")]
    public async Task UpdateMembershipType_Succeeds_WhenUserIsAuthorized()
    {
        // Log in as someone that has permission to update memberships:
        await LogInToDefaultLab(TestData.ManagerUser);

        // Attempt to update the current lab analyst to a manager:
        var response = await SendAsync(new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Manager
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the persisted information matches the request:
        Assert.Equal(LaboratoryMembershipType.Manager, await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.MemberId == TestData.MemberUser.Id && x.LaboratoryId == TestData.Lab.Id)
            .Select(x => x.MembershipType)
            .SingleAsync(AbortTest));
        
        // Reset the database because we messed with stuff:
        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[170] Update membership type endpoint returns 403 when user is not authorized")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsBasicUser()
    {
        // Log in as someone that does NOT have permission to update memberships:
        await LogInToDefaultLab(TestData.AnalystUser);

        // Attempt to update the current lab analyst to a manager:
        var response = await SendAsync(new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that the user's membership type did not change:
        Assert.Equal(LaboratoryMembershipType.Member, await DbContext.Memberships
            .IgnoreQueryFilters()
            .Where(x => x.MemberId == TestData.MemberUser.Id && x.LaboratoryId == TestData.Lab.Id)
            .Select(x => x.MembershipType)
            .SingleAsync(AbortTest));
    }
    
    [Fact(DisplayName = "[170] Update membership type endpoint returns 422 when member does not exist")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsNotAMember()
    {
        // Log in as someone that has permission to update memberships:
        await LogInToDefaultLab(TestData.ManagerUser);
        
        // Attempt to update a user that is not a member of the current lab:
        var response = await SendAsync(new UpdateMembershipRequest
        {
            MemberId = TestData.NonMemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact(DisplayName = "[170] Update membership type endpoint returns 422 when user is not a member of the lab")]
    public async Task UpdateMembership_ShouldFail_WhenUserDoesNotExist()
    {
        // Log in as someone that has permission to update memberships:
        await LogInToDefaultLab(TestData.ManagerUser);
        
        // Attempt to update a non-existent user:
        var response = await SendAsync(new UpdateMembershipRequest
        {
            MemberId = Guid.NewGuid(),
            MembershipType = LaboratoryMembershipType.Analyst
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
    
    [Fact(DisplayName = "[275] Get analysts succeeds when request is valid")]
    public async Task SucceedsWhenRequestIsValid_ExcludeHistoricAnalysts()
    {
        // Log in as someone that has permission to view lab information:
        await LogInToDefaultLab(TestData.MemberUser);
        
        // Attempt to get a list of analysts for the current lab:
        var response = await SendAsync(new GetAnalystsRequest { IncludeHistoricAnalysts = false });
        
        // Verify that this succeeds:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify that the response contains the expected members:
        var members = await response.Content.ReadFromJsonAsync<LaboratoryMemberDto[]>(AbortTest);
        Assert.NotNull(members);
        
        // All of these users can act as analysts, so they should be included:
        Assert.Contains(members, m => m.Id == TestData.AdminUser.Id);
        Assert.Contains(members, m => m.Id == TestData.ManagerUser.Id);
        Assert.Contains(members, m => m.Id == TestData.AnalystUser.Id);
        
        // This user is not an analyst, so they should not be included:
        Assert.DoesNotContain(members, m => m.Id == TestData.MemberUser.Id);
    }
    
    [Fact(DisplayName = "[275] Get analysts includes historic analysts when requested")]
    public async Task SucceedsWhenRequestIsValid_IncludeHistoricAnalysts()
    {
        // Historic analysts are users who have acted as analysts in the past, or currently have the analyst role (or higher).
        // Manually add a project with non-analyst user to the database:
        await AddDataAsync(new Project
        {
            Id = Guid.NewGuid(),
            LaboratoryId = TestData.Lab.Id,
            CustomerName = "Old Customer",
            ProjectCode = new ProjectCode("OLD", "200101", 1),
            CreatedById = TestData.AdminUser.Id,
            CreatedOn = new DateTime(2020, 1, 1),
            ProjectStatus = ProjectStatus.Completed,
            LeadAnalystId = TestData.MemberUser.Id
        });
        
        // Log in as someone that has permission to view lab information:
        await LogInToDefaultLab(TestData.MemberUser);
        
        // Attempt to get a list of analysts for the current lab, including historic analysts:
        var response = await SendAsync(new GetAnalystsRequest { IncludeHistoricAnalysts = true });
        
        // Verify that this succeeds:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // Verify that the response contains the expected members:
        var members = await response.Content.ReadFromJsonAsync<LaboratoryMemberDto[]>(AbortTest);
        Assert.NotNull(members);
        
        // All of these users can act as analysts, so they should be included:
        Assert.Contains(members, m => m.Id == TestData.AdminUser.Id);
        Assert.Contains(members, m => m.Id == TestData.ManagerUser.Id);
        Assert.Contains(members, m => m.Id == TestData.AnalystUser.Id);
        
        // This user is not currently an analyst, but they did act as one in the past, so they should be included:
        Assert.Contains(members, m => m.Id == TestData.MemberUser.Id);

        // Reset the database because we messed with stuff:
        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[275] Get analysts fails when user is not authorized")]
    public async Task FailsWhenUserIsNotAuthorized()
    {
        // Login as someone that does NOT have permission to view lab information:
        await LoginAs(TestData.NonMemberUser);
        
        // Attempt to view the list of analysts:
        var response = await SendAsync(new GetAnalystsRequest());
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}