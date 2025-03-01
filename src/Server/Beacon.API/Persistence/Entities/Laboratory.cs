using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Laboratory
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }

    public List<Membership> Memberships { get; set; } = [];
    public List<Project> Projects { get; set; } = [];

    public Membership AddMember(Guid userId, LaboratoryMembershipType membershipType = LaboratoryMembershipType.Member)
    {
        var membership = new Membership
        {
            LaboratoryId = Id,
            MemberId = userId,
            MembershipType = membershipType
        };

        Memberships.Add(membership);

        return membership;
    }

    public bool HasMember(User user) => Memberships.Any(m => m.MemberId == user.Id);
}