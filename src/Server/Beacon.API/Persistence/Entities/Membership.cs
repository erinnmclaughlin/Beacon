using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Membership : LaboratoryScopedEntityBase
{
    public required LaboratoryMembershipType MembershipType { get; set; }

    public required Guid MemberId { get; init; }
    public User Member { get; set; } = null!;
}
