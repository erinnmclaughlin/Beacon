using Beacon.Common.Models;

namespace Beacon.App.Entities;

public class Membership : LaboratoryScopedEntityBase
{
    public required LaboratoryMembershipType MembershipType { get; set; }

    public required Guid MemberId { get; init; }
    public User Member { get; set; } = null!;
}
