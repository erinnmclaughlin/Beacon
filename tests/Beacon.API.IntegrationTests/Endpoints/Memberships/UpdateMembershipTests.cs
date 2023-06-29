using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using System.Net;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class UpdateMembershipTests : TestBase
{
    public UpdateMembershipTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Update membership type returns 403 when user is not authorized")]
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

    [Fact(DisplayName = "Update membership type succeeds when user is authorized")]
    public async Task UpdateMembershipType_Succeeds_WhenUserIsAuthorized()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PutAsync("api/memberships", new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        ResetDatabase();
    }
}
