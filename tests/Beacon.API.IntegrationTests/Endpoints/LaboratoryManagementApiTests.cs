using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "API")]
[Trait("Category", "Laboratory Management")]
public sealed class LaboratoryManagementApiTests(TestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact(DisplayName = "[002] Create lab succeeds when request is valid")]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        // Log in as anyone:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to create a new lab:
        var response = await SendAsync(new CreateLaboratoryRequest { LaboratoryName = "My New Lab" });

        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the lab was persisted:
        var createdLab = await FindLabByNameWithMembers("My New Lab");
        Assert.NotNull(createdLab);
        
        // Verify that the current user was added as the lab's admin:
        var member = Assert.Single(createdLab.Memberships);
        Assert.Equal(TestData.NonMemberUser.Id, member.MemberId);
        Assert.Equal(LaboratoryMembershipType.Admin, member.MembershipType);

        // We messed with stuff, so reset the db:
        ShouldResetDatabase = true;
    }
    
    [Fact(DisplayName = "[002] Create lab fails when request is invalid")]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        // Log in as anyone:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to create a new lab with an invalid name (too short):
        var response = await SendAsync(new CreateLaboratoryRequest { LaboratoryName = "no" });

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        // Verify that the lab NOT persisted:
        var createdLab = await FindLabByNameWithMembers("no");
        Assert.Null(createdLab);
    }
    
    [Fact(DisplayName = "[185] Set current lab succeeds when request is valid")]
    public async Task SetCurrentLaboratory_SucceedsWhenRequestIsValid()
    {
        // Log in as a current lab member:
        await LoginAs(TestData.MemberUser);
        
        // Set the current lab to the default lab:
        var response = await SetCurrentLab(TestData.Lab.Id);

        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();
        
        // Verify that cookie is updated to reflect the current lab selection:
        Assert.True(response.Headers.Contains("Set-Cookie"));
    }
    
    [Fact(DisplayName = "[185] Set current lab fails when user is not a lab member")]
    public async Task SetCurrentLaboratory_FailsWhenUserIsNotAMember()
    {
        // Login as someone that's not a member of the default lab:
        await LoginAs(TestData.NonMemberUser);
        
        // Set the current lab to the default lab:
        var response = await SetCurrentLab(TestData.Lab.Id);

        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        // Verify that cookie is NOT updated:
        Assert.False(response.Headers.Contains("Set-Cookie"));
    }
    
    [Fact(DisplayName = "[185] Get current lab endpoint returns lab info if current user is a member")]
    public async Task GetCurrentLab_ReturnsExpectedResult()
    {
        // Log in as a current lab member:
        await LogInToDefaultLab(TestData.MemberUser);

        // Attempt to get the current lab:
        var response = await SendAsync(new GetCurrentLaboratoryRequest());
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the response content contains the expected information:
        var lab = await response.Content.ReadFromJsonAsync<LaboratoryDto>(AbortTest);
        Assert.NotNull(lab);
        Assert.Equal(TestData.Lab.Id, lab.Id);
        Assert.Equal(TestData.Lab.Name, lab.Name);
    }
    
    [Fact(DisplayName = "[185] Get current lab endpoint does not return lab info if current user is not a member")]
    public async Task GetCurrentLab_ReturnsForbidden_IfCurrentUserIsNotAuthorized()
    {
        // Log in as someone that's not in a lab:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to get the current lab:
        var response = await SendAsync(new GetCurrentLaboratoryRequest());
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    
    [Fact(DisplayName = "[185] Get my labs returns current user's labs only")]
    public async Task GetMyLaboratories_ReturnsOnlyCurrentUsersLabs()
    {
        // Log in as someone that's currently a member of the default lab:
        await LoginAs(TestData.MemberUser);
        
        // Create a new lab while logged in as that user:
        await SendAsync(new CreateLaboratoryRequest
        {
            LaboratoryName = "The Other Lab"
        });
        
        // Get my labs:
        var getMyLabsResponse = await SendAsync(new GetMyLaboratoriesRequest());
        var myLabs = await getMyLabsResponse.Content.ReadFromJsonAsync<LaboratoryDto[]>(AbortTest);
        
        // Verify that I now belong to two labs (the default lab and the new lab):
        Assert.NotNull(myLabs);
        Assert.Equal(2, myLabs.Length);
        Assert.Contains(myLabs, x => x.Name == "The Other Lab");

        // Now log in as someone else:
        await LoginAs(TestData.ManagerUser);
        getMyLabsResponse = await SendAsync(new GetMyLaboratoriesRequest());
        myLabs = await getMyLabsResponse.Content.ReadFromJsonAsync<LaboratoryDto[]>(AbortTest);
        
        // Verify that they still just belong to the default lab:
        Assert.NotNull(myLabs);
        Assert.Single(myLabs);
        Assert.DoesNotContain(myLabs, x => x.Name == "The Other Lab");
        
        // We messed with stuff, so reset the db:
        ShouldResetDatabase = true;
    }
    
    private Task<Laboratory?> FindLabByNameWithMembers(string labName) => DbContext.Laboratories
        .IgnoreQueryFilters()
        .Include(x => x.Memberships)
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.Name == labName);
}