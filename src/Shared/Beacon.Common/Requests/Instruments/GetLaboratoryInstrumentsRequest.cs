using Beacon.Common.Models;

namespace Beacon.Common.Requests.Instruments;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetLaboratoryInstrumentsRequest : BeaconRequest<GetLaboratoryInstrumentsRequest, LaboratoryInstrumentDto[]>
{
    public List<Guid>? IgnoredInstrumentIds { get; set; }
}
