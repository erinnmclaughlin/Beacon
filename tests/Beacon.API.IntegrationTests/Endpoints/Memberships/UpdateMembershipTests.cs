using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class UpdateMembershipTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public UpdateMembershipTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact(DisplayName = "Basic members cannot update membership types.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsBasicUser()
    {
        var client = _factory.CreateClient(TestData.MemberUser.Id, TestData.Lab.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUserAlt.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        var response = await client.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory analysts cannot update membership types.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsAnalyst()
    {
        var client = _factory.CreateClient(TestData.AnalystUser.Id, TestData.Lab.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        var response = await client.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory managers can manage permissions for non-admin users.")]
    public async Task UpdateMembership_ShouldReturnExpectedResult_WhenUserIsManager()
    {
        var client = _factory.CreateClient(TestData.ManagerUser.Id, TestData.Lab.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.ManagerUserAlt.Id,
            MembershipType = LaboratoryMembershipType.Member
        };

        var response = await client.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory managers cannot manage permissions of admin users.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsManagerAndRequestIsForAdmin()
    {
        var client = _factory.CreateClient(TestData.ManagerUser.Id, TestData.Lab.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Member
        };

        var response = await client.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory managers cannot change membership type to admin.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsManagerAndRequestIsForNewAdmin()
    {
        var client = _factory.CreateClient(TestData.ManagerUser.Id, TestData.Lab.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.ManagerUserAlt.Id,
            MembershipType = LaboratoryMembershipType.Admin
        };

        var response = await client.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Users cannot update their own membership type.")]
    public async Task UsersCannotEditTheirOwnMemberships()
    {
        var client = _factory.CreateClient(TestData.AdminUser.Id, TestData.Lab.Id);
        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Manager
        };

        var response = await client.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static Guid GetAltUserId(LaboratoryMembershipType membershipType) => membershipType switch
    {
        LaboratoryMembershipType.Member => TestData.MemberUserAlt.Id,
        LaboratoryMembershipType.Analyst => TestData.AnalystUserAlt.Id,
        LaboratoryMembershipType.Manager => TestData.ManagerUserAlt.Id,
        LaboratoryMembershipType.Admin => TestData.AdminUserAlt.Id,
        _ => throw new UnreachableException()
    };
}
