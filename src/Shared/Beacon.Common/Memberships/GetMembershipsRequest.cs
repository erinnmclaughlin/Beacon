using MediatR;

namespace Beacon.Common.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public class GetMembershipsRequest : IRequest<LaboratoryMemberDto[]>
{
}
