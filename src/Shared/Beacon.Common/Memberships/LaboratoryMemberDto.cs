namespace Beacon.Common.Memberships;

public record LaboratoryMemberDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required string EmailAddress { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }
}
