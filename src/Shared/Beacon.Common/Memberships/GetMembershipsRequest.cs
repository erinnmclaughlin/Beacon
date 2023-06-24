using MediatR;

namespace Beacon.Common.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public class GetMembershipsRequest : LaboratoryRequestBase, IRequest<LaboratoryMemberDto[]>
{
}
