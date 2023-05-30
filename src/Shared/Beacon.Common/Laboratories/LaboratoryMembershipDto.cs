namespace Beacon.Common.Laboratories;

public record LaboratoryMembershipDto
{
    public required Guid MemberId { get; init; }
    public required string MemberDisplayName { get; init; }
    public required string MembershipType { get; init; }

    public required Guid LaboratoryId { get; init; }
    public required string LaboratoryName { get; init; }
}
