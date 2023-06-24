using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public class GetLaboratoryByIdRequest : LaboratoryRequestBase, IRequest<LaboratoryDto>
{
}
