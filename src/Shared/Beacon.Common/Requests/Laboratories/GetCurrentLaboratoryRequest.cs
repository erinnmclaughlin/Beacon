using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetCurrentLaboratoryRequest : BeaconRequest<GetCurrentLaboratoryRequest>, IRequest<LaboratoryDto> { }
