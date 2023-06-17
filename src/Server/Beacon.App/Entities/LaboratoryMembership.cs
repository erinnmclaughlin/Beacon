using Beacon.Common.Laboratories.Enums;

namespace Beacon.App.Entities;

public class LaboratoryMembership : LaboratoryScopedEntityBase
{
    public required LaboratoryMembershipType MembershipType { get; set; }

    public required Guid MemberId { get; init; }
    public User Member { get; set; } = null!;
}
