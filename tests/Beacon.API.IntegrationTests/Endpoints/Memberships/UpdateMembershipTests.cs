using Beacon.App.Exceptions;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class UpdateMembershipTests : TestBase
{
    public UpdateMembershipTests(TestFixture testFixture) : base(testFixture)
    {
    }

    [Fact(DisplayName = "Basic members cannot update membership types.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsBasicUser()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.AnalystUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        await Assert.ThrowsAsync<UserNotAllowedException>(() => SendAsync(request));
    }

    [Fact(DisplayName = "Laboratory analysts cannot update membership types.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsAnalyst()
    {
        SetCurrentUser(TestData.AnalystUser.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        await Assert.ThrowsAsync<UserNotAllowedException>(() => SendAsync(request));
    }

    [Fact(DisplayName = "Laboratory managers can manage permissions for non-admin users.")]
    public async Task UpdateMembership_ShouldReturnExpectedResult_WhenUserIsManager()
    {
        SetCurrentUser(TestData.ManagerUser.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        await SendAsync(request);
    }

    [Fact(DisplayName = "Laboratory managers cannot manage permissions of admin users.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsManagerAndRequestIsForAdmin()
    {
        SetCurrentUser(TestData.ManagerUser.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.AdminUser.Id,
            MembershipType = LaboratoryMembershipType.Member
        };

        await Assert.ThrowsAsync<UserNotAllowedException>(() => SendAsync(request));
    }

    [Fact(DisplayName = "Laboratory managers cannot change membership type to admin.")]
    public async Task UpdateMembership_ShouldFail_WhenUserIsManagerAndRequestIsForNewAdmin()
    {
        SetCurrentUser(TestData.ManagerUser.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.MemberUser.Id,
            MembershipType = LaboratoryMembershipType.Admin
        };

        await Assert.ThrowsAsync<UserNotAllowedException>(() => SendAsync(request));
    }

    [Fact(DisplayName = "Users cannot update their own membership type.")]
    public async Task UsersCannotEditTheirOwnMemberships()
    {
        SetCurrentUser(TestData.ManagerUser.Id);

        var request = new UpdateMembershipRequest
        {
            MemberId = TestData.ManagerUser.Id,
            MembershipType = LaboratoryMembershipType.Analyst
        };

        await Assert.ThrowsAsync<UserNotAllowedException>(() => SendAsync(request));
    }
}
