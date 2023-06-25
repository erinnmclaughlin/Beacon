using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Manager)]
public sealed class UpdateMembershipRequest : IRequest
{
    public required Guid MemberId { get; set; }
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;
}
