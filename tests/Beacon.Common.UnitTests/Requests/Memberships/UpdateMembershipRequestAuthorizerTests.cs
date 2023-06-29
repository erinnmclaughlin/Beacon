using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;
using Beacon.Common.Services;
using Moq;
using Xunit;

namespace Beacon.Common.UnitTests.Requests.Memberships;

public sealed class UpdateMembershipRequestAuthorizerTests
{
    private readonly ICurrentUser _currentUser;
    private readonly Mock<ILabContext> _labContextMock = new();

    public UpdateMembershipRequestAuthorizerTests()
    {
        var currentUserMock = new Mock<ICurrentUser>();
        currentUserMock.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUser = currentUserMock.Object;
    }

    [Fact]
    public async Task AuthorizeAsync_ReturnsFalse_WhenCurrentUserMatchesRequest()
    {
        var request = new UpdateMembershipRequest { MemberId = _currentUser.UserId };

        var sut = new UpdateMembershipRequest.Authorizer(_currentUser, _labContextMock.Object);
        var result = await sut.IsAuthorizedAsync(request);

        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(LaboratoryMembershipType.Analyst)]
    [InlineData(LaboratoryMembershipType.Member)]
    public async Task AuthorizeAsync_ReturnsFalse_WhenUserIsNotAdminOrManager(LaboratoryMembershipType? type)
    {
        _labContextMock
            .Setup(x => x.GetMembershipTypeAsync(_currentUser.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LaboratoryMembershipType?)null);

        var request = new UpdateMembershipRequest { MemberId = Guid.NewGuid() };

        var sut = new UpdateMembershipRequest.Authorizer(_currentUser, _labContextMock.Object);
        var result = await sut.IsAuthorizedAsync(request);

        Assert.False(result);
    }

    [Theory]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Manager)]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Analyst)]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Member)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Admin)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Analyst)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Member)]
    [InlineData(LaboratoryMembershipType.Analyst, LaboratoryMembershipType.Admin)]
    [InlineData(LaboratoryMembershipType.Analyst, LaboratoryMembershipType.Manager)]
    [InlineData(LaboratoryMembershipType.Analyst, LaboratoryMembershipType.Member)]
    [InlineData(LaboratoryMembershipType.Member, LaboratoryMembershipType.Admin)]
    [InlineData(LaboratoryMembershipType.Member, LaboratoryMembershipType.Manager)]
    [InlineData(LaboratoryMembershipType.Member, LaboratoryMembershipType.Analyst)]
    public async Task AuthorizeAsync_ReturnsExpectedResult_WhenCurrentUserIsAdmin(LaboratoryMembershipType oldType, LaboratoryMembershipType newType)
    {
        var memberId = Guid.NewGuid();

        _labContextMock
            .Setup(x => x.GetMembershipTypeAsync(_currentUser.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(LaboratoryMembershipType.Admin);

        _labContextMock
            .Setup(x => x.GetMembershipTypeAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldType);

        var sut = new UpdateMembershipRequest.Authorizer(_currentUser, _labContextMock.Object);
        var result = await sut.IsAuthorizedAsync(new() { MemberId = memberId, MembershipType = newType });

        Assert.True(result);
    }

    [Theory]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Manager, false)]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Analyst, false)]
    [InlineData(LaboratoryMembershipType.Admin, LaboratoryMembershipType.Member, false)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Admin, false)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Analyst, true)]
    [InlineData(LaboratoryMembershipType.Manager, LaboratoryMembershipType.Member, true)]
    [InlineData(LaboratoryMembershipType.Analyst, LaboratoryMembershipType.Admin, false)]
    [InlineData(LaboratoryMembershipType.Analyst, LaboratoryMembershipType.Manager, true)]
    [InlineData(LaboratoryMembershipType.Analyst, LaboratoryMembershipType.Member, true)]
    [InlineData(LaboratoryMembershipType.Member, LaboratoryMembershipType.Admin, false)]
    [InlineData(LaboratoryMembershipType.Member, LaboratoryMembershipType.Manager, true)]
    [InlineData(LaboratoryMembershipType.Member, LaboratoryMembershipType.Analyst, true)]
    public async Task AuthorizeAsync_ReturnsExpectedResult_WhenCurrentUserIsManager(LaboratoryMembershipType oldType, LaboratoryMembershipType newType, bool isAllowed)
    {
        var memberId = Guid.NewGuid();

        _labContextMock
            .Setup(x => x.GetMembershipTypeAsync(_currentUser.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(LaboratoryMembershipType.Manager);

        _labContextMock
            .Setup(x => x.GetMembershipTypeAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(oldType);

        var sut = new UpdateMembershipRequest.Authorizer(_currentUser, _labContextMock.Object);
        var result = await sut.IsAuthorizedAsync(new() { MemberId = memberId, MembershipType = newType });

        Assert.Equal(isAllowed, result);
    }
}
