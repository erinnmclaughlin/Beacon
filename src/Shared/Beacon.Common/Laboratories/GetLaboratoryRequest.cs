using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public class GetLaboratoryRequest : IRequest<LaboratoryDto> { }
