using Beacon.API.Endpoints.Invitations;
using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Services;

namespace Beacon.API.IntegrationTests.Endpoints.Invitations;

[Trait("Feature", "User Management")]
public sealed class CreateInvitationAuthorizerTests
{
    [Theory]
    [InlineData(LaboratoryMembershipType.Member)]
    [InlineData(LaboratoryMembershipType.Analyst)]
    [InlineData(LaboratoryMembershipType.Manager)]
    [InlineData(LaboratoryMembershipType.Admin)]
    public async Task Authorizer_ReturnsTrue_WhenCurrentUserIsAdmin(LaboratoryMembershipType newMemberType)
    {
        var mock = new Mock<ICurrentUser>();
        mock.SetupGet(x => x.MembershipType).Returns(LaboratoryMembershipType.Admin);

        var request = new CreateEmailInvitationRequest { MembershipType = newMemberType, NewMemberEmailAddress = "something@idk.com" };

        var sut = new CreateEmailInvitation.Authorizer(mock.Object);

        Assert.True(await sut.IsAuthorizedAsync(request));
    }

    [Theory]
    [InlineData(LaboratoryMembershipType.Member, true)]
    [InlineData(LaboratoryMembershipType.Analyst, true)]
    [InlineData(LaboratoryMembershipType.Manager, true)]
    [InlineData(LaboratoryMembershipType.Admin, false)]
    public async Task Authorizer_ReturnsExpectedResult_WhenCurrentUserIsManager(LaboratoryMembershipType newMemberType, bool shouldBeAllowed)
    {
        var mock = new Mock<ICurrentUser>();
        mock.SetupGet(x => x.MembershipType).Returns(LaboratoryMembershipType.Manager);

        var request = new CreateEmailInvitationRequest { MembershipType = newMemberType, NewMemberEmailAddress = "something@idk.com" };

        var sut = new CreateEmailInvitation.Authorizer(mock.Object);

        Assert.Equal(shouldBeAllowed, await sut.IsAuthorizedAsync(request));
    }
}
