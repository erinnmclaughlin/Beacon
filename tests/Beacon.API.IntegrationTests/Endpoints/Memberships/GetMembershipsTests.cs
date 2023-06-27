using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Models;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class GetMembershipsTests : TestBase
{
    public GetMembershipsTests(ApiFactory factory) : base(factory) { }

    [Fact(DisplayName = "Unauthorized users cannot access laboratory membership list")]
    public async Task GetMemberships_FailsWhenUserIsNotAMember()
    {
        SetCurrentUser(TestData.NonMemberUser);

        var response = await _httpClient.GetAsync("api/memberships");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Authorized users can access laboratory membership list")]
    public async Task GetMemberships_ReturnsExpectedResult_WhenUserIsMember()
    {
        var response = await _httpClient.GetAsync("api/memberships");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var memberships = await response.Content.ReadFromJsonAsync<LaboratoryMemberDto[]>(JsonDefaults.JsonSerializerOptions);

        Assert.NotNull(memberships);
        Assert.NotEmpty(memberships);
        Assert.Contains(memberships, m => m.Id == TestData.AdminUser.Id && m.MembershipType == LaboratoryMembershipType.Admin);
        Assert.Contains(memberships, m => m.Id == TestData.ManagerUser.Id && m.MembershipType == LaboratoryMembershipType.Manager);
        Assert.Contains(memberships, m => m.Id == TestData.AnalystUser.Id && m.MembershipType == LaboratoryMembershipType.Analyst);
        Assert.Contains(memberships, m => m.Id == TestData.MemberUser.Id && m.MembershipType == LaboratoryMembershipType.Member);
        Assert.DoesNotContain(memberships, m => m.Id == TestData.NonMemberUser.Id);
    }
}
