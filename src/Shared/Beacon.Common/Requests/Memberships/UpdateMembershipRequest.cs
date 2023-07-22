using Beacon.Common.Models;

namespace Beacon.Common.Requests.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Manager)]
public sealed class UpdateMembershipRequest : BeaconRequest<UpdateMembershipRequest>
{
    public required Guid MemberId { get; set; }
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;
}
