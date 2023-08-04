namespace DataImporter.Entities;

public sealed class Membership
{
    public required string MembershipType { get; set; }

    public Guid LaboratoryId { get; init; }
    public Laboratory Laboratory { get; init; } = default!;

    public Guid MemberId { get; init; }
    public User Member { get; set; } = default!;
}
