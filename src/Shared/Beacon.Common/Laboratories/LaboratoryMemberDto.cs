namespace Beacon.Common.Laboratories;

public record LaboratoryMemberDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public UserDto ToUserDto() => new()
    {
        Id = Id,
        DisplayName = DisplayName,
        EmailAddress = EmailAddress
    };
}
