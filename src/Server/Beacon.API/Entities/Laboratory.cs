namespace Beacon.API.Entities;

public class Laboratory
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }

    public List<LaboratoryMembership> Memberships { get; set; } = new();

    public LaboratoryMembership AddMember(User member, LaboratoryMembershipType membershipType)
    {
        var membership = new LaboratoryMembership
        {
            Laboratory = this,
            Member = member,
            MembershipType = membershipType
        };

        Memberships.Add(membership);

        return membership;
    }
}