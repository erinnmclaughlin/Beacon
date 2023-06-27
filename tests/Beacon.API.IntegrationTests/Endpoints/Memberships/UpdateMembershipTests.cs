using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class UpdateMembershipTests : TestBase
{
    public UpdateMembershipTests(ApiFactory factory) : base(factory) { }

    [Fact(DisplayName = "Basic members cannot update membership types.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsBasicUser()
    {
        SetCurrentUser(TestData.MemberUser);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.AnalystUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        var response = await _httpClient.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory analysts cannot update membership types.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsAnalyst()
    {
        SetCurrentUser(TestData.AnalystUser);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        var response = await _httpClient.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory managers can manage permissions for non-admin users.")]
    public async Task UpdateMembership_ShouldReturnExpectedResult_WhenUserIsManager()
    {
        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        var response = await _httpClient.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory managers cannot manage permissions of admin users.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsManagerAndRequestIsForAdmin()
    {
        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Member
        };

        var response = await _httpClient.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Laboratory managers cannot change membership type to admin.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsManagerAndRequestIsForNewAdmin()
    {
        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Admin
        };

        var response = await _httpClient.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact(DisplayName = "Users cannot update their own membership type.")]
    public async Task UsersCannotEditTheirOwnMemberships()
    {
        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Manager
        };

        var response = await _httpClient.PutAsJsonAsync("api/memberships", request, JsonDefaults.JsonSerializerOptions);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
