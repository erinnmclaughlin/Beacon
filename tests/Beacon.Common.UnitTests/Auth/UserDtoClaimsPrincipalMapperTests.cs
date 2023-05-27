using Beacon.Common.Auth;
using System.Security.Claims;

namespace Beacon.Common.UnitTests.Auth;

public sealed class UserDtoClaimsPrincipalMapperTests
{
    [Fact]
    public void Mapper_ReturnsAnonymousClaimsPrincipal_WhenUserIsNull()
    {
        var cp = UserDtoClaimsPrincipalMapper.ToClaimsPrincipal(null);
        cp.Identities.Should().ContainSingle().Which.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void Mapper_ReturnsExpectedClaimsPrincipal_WhenUserIsNotNull()
    {
        var someUser = new UserDto
        {
            Id = Guid.NewGuid(),
            EmailAddress = "someone@test.com",
            DisplayName = "Test User"
        };

        var cp = UserDtoClaimsPrincipalMapper.ToClaimsPrincipal(someUser);

        cp.Identities.Should().ContainSingle().Which.IsAuthenticated.Should().BeTrue();
        cp.FindAll(ClaimTypes.NameIdentifier).Should().ContainSingle().Which.Value.Should().Be(someUser.Id.ToString());
        cp.FindAll(ClaimTypes.Name).Should().ContainSingle().Which.Value.Should().Be(someUser.DisplayName);
        cp.FindAll(ClaimTypes.Email).Should().ContainSingle().Which.Value.Should().Be(someUser.EmailAddress);
    }

    [Fact]
    public void Mapper_ReturnsNullUser_WhenNotAuthenticated()
    {
        var anonymousClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

        var user = UserDtoClaimsPrincipalMapper.ToUserDto(anonymousClaimsPrincipal);

        user.Should().BeNull();
    }

    [Fact]
    public void Mapper_ReturnsExpectedUser_WhenAuthenticated()
    {
        var id = Guid.NewGuid();
        var name = "Test";
        var email = "someone@test.com";

        var identity = new ClaimsIdentity("TestAuth");
        identity.AddClaims(new Claim[]
        {
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email)
        });

        var user = UserDtoClaimsPrincipalMapper.ToUserDto(new(identity));

        user.Should().NotBeNull().As<UserDto>();
        user!.Id.Should().Be(id);
        user.DisplayName.Should().Be(name);
        user.EmailAddress.Should().Be(email);
    }
}
