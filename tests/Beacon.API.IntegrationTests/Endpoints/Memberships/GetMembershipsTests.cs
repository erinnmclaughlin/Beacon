using Beacon.Common;
using Beacon.Common.Models;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

[Collection(ApiTest.Name)]
public sealed class GetMembershipsTests
{
    private readonly ApiFactory _factory;

    public GetMembershipsTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Unauthorized users cannot access laboratory membership list")]
    public async Task GetMemberships_FailsWhenUserIsNotAMember()
    {
        var client = _factory.CreateClient(TestData.NonMemberUser.Id, TestData.Lab.Id);
        var response = await client.GetAsync("api/memberships");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Authorized users can access laboratory membership list")]
    public async Task GetMemberships_ReturnsExpectedResult_WhenUserIsMember()
    {
        var client = _factory.CreateClient(TestData.AdminUser.Id, TestData.Lab.Id);
        var response = await client.GetAsync("api/memberships");
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
