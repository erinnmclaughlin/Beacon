using Beacon.Common.Laboratories.Enums;

namespace Beacon.API.Domain.Entities;

public class LaboratoryMembership
{
    public required LaboratoryMembershipType MembershipType { get; set; }

    public required Guid LaboratoryId { get; set; }
    public Laboratory Laboratory { get; set; } = null!;

    public required Guid MemberId { get; set; }
    public User Member { get; set; } = null!;
}
