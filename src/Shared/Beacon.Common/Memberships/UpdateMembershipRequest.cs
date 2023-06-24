using MediatR;

namespace Beacon.Common.Memberships;

public class UpdateMembershipRequest : IRequest
{
    public required Guid LaboratoryId { get; set; }
    public required Guid MemberId { get; set; }
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;
}
