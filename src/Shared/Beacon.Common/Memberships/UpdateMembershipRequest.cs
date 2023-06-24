using MediatR;

namespace Beacon.Common.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Manager)]
public class UpdateMembershipRequest : LaboratoryRequestBase, IRequest
{
    public required Guid MemberId { get; set; }
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;
}
