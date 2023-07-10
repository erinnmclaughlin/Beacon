using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

[Trait("Feature", "User Management")]
public sealed class GetMembershipsTests : TestBase
{
    public GetMembershipsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[170] Get memberships endpoint returns 403 when user is not authorized")]
    public async Task GetMemberships_FailsWhenUserIsNotAMember()
    {
        RunAsNonMember();

        var response = await GetAsync("api/memberships");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "[170] Get memberships endpoint returns list of lab members when user is authorized")]
    public async Task GetMemberships_ReturnsExpectedResult_WhenUserIsMember()
    {
        RunAsMember();

        var response = await GetAsync("api/memberships");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var memberships = await DeserializeAsync<LaboratoryMemberDto[]>(response);

        Assert.NotNull(memberships);
        Assert.Contains(memberships, m => m.Id == TestData.AdminUser.Id && m.MembershipType == LaboratoryMembershipType.Admin);
        Assert.Contains(memberships, m => m.Id == TestData.ManagerUser.Id && m.MembershipType == LaboratoryMembershipType.Manager);
        Assert.Contains(memberships, m => m.Id == TestData.AnalystUser.Id && m.MembershipType == LaboratoryMembershipType.Analyst);
        Assert.Contains(memberships, m => m.Id == TestData.MemberUser.Id && m.MembershipType == LaboratoryMembershipType.Member);
        Assert.DoesNotContain(memberships, m => m.Id == TestData.NonMemberUser.Id);
    }
}
