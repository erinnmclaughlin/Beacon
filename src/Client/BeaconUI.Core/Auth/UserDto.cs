namespace BeaconUI.Core.Auth;

public record UserDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}
