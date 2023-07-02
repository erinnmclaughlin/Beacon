using Beacon.API.IntegrationTests.Collections;
using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class UpdateMembershipTests : CoreTestBase
{
    public UpdateMembershipTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[170] Update membership type succeeds when user is authorized")]
    public async Task UpdateMembershipType_Succeeds_WhenUserIsAuthorized()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PutAsync("api/memberships", new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        var membership = await db.Memberships
            .SingleOrDefaultAsync(m => m.MemberId == TestData.MemberUser.Id && m.LaboratoryId == TestData.Lab.Id);

        Assert.Equal(LaboratoryMembershipType.Analyst, membership?.MembershipType);
    }

    [Fact(DisplayName = "[170] Update membership type endpoint returns 403 when user is not authorized")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsBasicUser()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PutAsync("api/memberships", new UpdateMembershipRequest
        {
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
