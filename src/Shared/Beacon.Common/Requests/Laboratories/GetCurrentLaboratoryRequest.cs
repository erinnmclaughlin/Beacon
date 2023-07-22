using Beacon.Common.Models;

namespace Beacon.Common.Requests.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetCurrentLaboratoryRequest : BeaconRequest<GetCurrentLaboratoryRequest, LaboratoryDto> { }
