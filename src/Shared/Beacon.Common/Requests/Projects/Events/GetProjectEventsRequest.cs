using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects.Events;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectEventsRequest : BeaconRequest<GetProjectEventsRequest, ProjectEventDto[]>
{
    public Guid ProjectId { get; set; }
    public DateTime? MinStart { get; set; }
    public DateTime? MaxStart { get; set; }
    public DateTime? MinEnd { get; set; }
    public DateTime? MaxEnd { get; set; }
}
