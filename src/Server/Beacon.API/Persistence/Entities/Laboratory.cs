using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Laboratory
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }

    private readonly List<Membership> _memberships = new();
    public IReadOnlyList<Membership> Memberships => _memberships;

    public Membership AddMember(Guid userId, LaboratoryMembershipType membershipType = LaboratoryMembershipType.Member)
    {
        var membership = new Membership
        {
            LaboratoryId = Id,
            MemberId = userId,
            MembershipType = membershipType
        };

        _memberships.Add(membership);

        return membership;
    }

    public bool HasMember(User user) => _memberships.Any(m => m.MemberId == user.Id);
}