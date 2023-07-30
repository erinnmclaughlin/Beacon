using Beacon.Common.Models;

namespace Beacon.Common.Requests.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetLaboratoryEventsRequest : BeaconRequest<GetLaboratoryEventsRequest, LaboratoryEventDto[]>
{
    public List<Guid> ProjectIds { get; set; } = new();
    public DateTime? MinStart { get; set; }
    public DateTime? MaxStart { get; set; }
    public DateTime? MinEnd { get; set; }
    public DateTime? MaxEnd { get; set; }
}
