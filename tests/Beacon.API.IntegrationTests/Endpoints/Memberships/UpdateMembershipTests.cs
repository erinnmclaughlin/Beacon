using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

[Trait("Feature", "User Management")]
public sealed class UpdateMembershipTests : TestBase
{
    public UpdateMembershipTests(TestFixture fixture) : base(fixture)
    {
    }

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

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        var membership = await db.Memberships
            .SingleOrDefaultAsync(m => m.MemberId == TestData.MemberUser.Id && m.LaboratoryId == TestData.Lab.Id);

        Assert.Equal(LaboratoryMembershipType.Analyst, membership?.MembershipType);
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
    }
}
