using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

[Trait("Feature", "User Management")]
public sealed class UpdateMembershipTests(TestFixture fixture) : TestBase(fixture)
{
    [Fact(DisplayName = "[170] Update membership type succeeds when user is authorized")]
    public async Task UpdateMembershipType_Succeeds_WhenUserIsAuthorized()
    {
        RunAsAdmin();

        var response = await SendAsync(new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        Assert.Equal(LaboratoryMembershipType.Analyst, await ExecuteDbContextAsync(async db =>
        {
            return await db.Memberships
                .Where(x => x.MemberId == TestData.MemberUser.Id && x.LaboratoryId == TestData.Lab.Id)
                .Select(x => x.MembershipType)
                .SingleAsync();
        }));
    }

    [Fact(DisplayName = "[170] Update membership type endpoint returns 403 when user is not authorized")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsBasicUser()
    {
        RunAsMember();

        var response = await SendAsync(new UpdateMembershipRequest
        {
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        
        Assert.Equal(LaboratoryMembershipType.Admin, await ExecuteDbContextAsync(async db =>
        {
            return await db.Memberships
                .Where(x => x.MemberId == TestData.AdminUser.Id && x.LaboratoryId == TestData.Lab.Id)
                .Select(x => x.MembershipType)
                .SingleAsync();
        }));
    }

    [Fact(DisplayName = "[170] Update membership type endpoint returns 422 when member does not exist.")]
    public async Task UpdateMembership_ShouldFail_WhenUserDoesNotExist()
    {
        RunAsAdmin();
        
        var response = await SendAsync(new UpdateMembershipRequest
        {
            MemberId = Guid.NewGuid(),
            MembershipType = LaboratoryMembershipType.Analyst
        });
        
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
}
