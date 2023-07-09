using Beacon.Common.Models;
using Beacon.Common.Requests.Invitations;
using Beacon.Common.Services;
using Moq;
using Xunit;

namespace Beacon.Common.UnitTests.Requests.Invitations;

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
        var labContextMock = new Mock<ILabContext>();
        labContextMock
            .Setup(x => x.GetMembershipTypeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(LaboratoryMembershipType.Admin);

        var request = new CreateEmailInvitationRequest { MembershipType = newMemberType, NewMemberEmailAddress = "something@idk.com" };

        var sut = new CreateEmailInvitationRequest.Authorizer(Mock.Of<ICurrentUser>(), labContextMock.Object);

        Assert.True(await sut.IsAuthorizedAsync(request));
    }

    [Theory]
    [InlineData(LaboratoryMembershipType.Member, true)]
    [InlineData(LaboratoryMembershipType.Analyst, true)]
    [InlineData(LaboratoryMembershipType.Manager, true)]
    [InlineData(LaboratoryMembershipType.Admin, false)]
    public async Task Authorizer_ReturnsExpectedResult_WhenCurrentUserIsManager(LaboratoryMembershipType newMemberType, bool shouldBeAllowed)
    {
        var labContextMock = new Mock<ILabContext>();
        labContextMock
            .Setup(x => x.GetMembershipTypeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(LaboratoryMembershipType.Manager);

        var request = new CreateEmailInvitationRequest { MembershipType = newMemberType, NewMemberEmailAddress = "something@idk.com" };

        var sut = new CreateEmailInvitationRequest.Authorizer(Mock.Of<ICurrentUser>(), labContextMock.Object);

        Assert.Equal(shouldBeAllowed, await sut.IsAuthorizedAsync(request));
    }
}
