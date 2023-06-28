using Beacon.App.Exceptions;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class GetMembershipsTests : TestBase
{
    public GetMembershipsTests(TestFixture testFixture) : base(testFixture)
    {
    }

    [Fact(DisplayName = "Unauthorized users cannot access laboratory membership list")]
    public async Task GetMemberships_FailsWhenUserIsNotAMember()
    {
        SetCurrentUser(TestData.NonMemberUser.Id);
        await Assert.ThrowsAsync<UserNotAllowedException>(() => SendAsync(new GetMembershipsRequest()));
    }

    [Fact(DisplayName = "Authorized users can access laboratory membership list")]
    public async Task GetMemberships_ReturnsExpectedResult_WhenUserIsMember()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var memberships = await SendAsync(new GetMembershipsRequest());

        Assert.NotEmpty(memberships);
        Assert.Contains(memberships, m => m.Id == TestData.AdminUser.Id && m.MembershipType == LaboratoryMembershipType.Admin);
        Assert.Contains(memberships, m => m.Id == TestData.ManagerUser.Id && m.MembershipType == LaboratoryMembershipType.Manager);
        Assert.Contains(memberships, m => m.Id == TestData.AnalystUser.Id && m.MembershipType == LaboratoryMembershipType.Analyst);
        Assert.Contains(memberships, m => m.Id == TestData.MemberUser.Id && m.MembershipType == LaboratoryMembershipType.Member);
        Assert.DoesNotContain(memberships, m => m.Id == TestData.NonMemberUser.Id);
    }
}
