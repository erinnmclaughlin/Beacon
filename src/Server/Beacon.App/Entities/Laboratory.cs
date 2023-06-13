using Beacon.Common.Laboratories.Enums;

namespace Beacon.App.Entities;

public class Laboratory
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }

    private readonly List<LaboratoryMembership> _memberships = new();
    public IReadOnlyList<LaboratoryMembership> Memberships => _memberships;

    private Laboratory() { }

    public static Laboratory CreateNew(string name, User admin)
    {
        return CreateNew(Guid.NewGuid(), name, admin);
    }

    public static Laboratory CreateNew(Guid id, string name, User admin)
    {
        var laboratory = new Laboratory
        {
            Id = id,
            Name = name
        };

        laboratory.AddMember(admin, LaboratoryMembershipType.Admin);

        return laboratory;
    }

    public LaboratoryMembership AddMember(User member, LaboratoryMembershipType membershipType = LaboratoryMembershipType.Member)
    {
        var membership = new LaboratoryMembership
        {
            LaboratoryId = Id,
            MemberId = member.Id,
            MembershipType = membershipType
        };

        _memberships.Add(membership);

        return membership;
    }
}