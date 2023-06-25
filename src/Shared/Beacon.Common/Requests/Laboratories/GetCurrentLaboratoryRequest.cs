using Beacon.Common.Laboratories;
using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Requests.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public class GetCurrentLaboratoryRequest : IRequest<LaboratoryDto> { }
