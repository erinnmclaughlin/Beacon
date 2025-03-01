using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Membership : LaboratoryScopedEntityBase
{
    public LaboratoryMembershipType MembershipType { get; set; }

    public Guid MemberId { get; init; }
    public User Member { get; init; } = null!;
}
