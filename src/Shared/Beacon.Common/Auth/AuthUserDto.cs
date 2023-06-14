using Beacon.Common.Laboratories;

namespace Beacon.Common.Auth;

public class AuthUserDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }
}
