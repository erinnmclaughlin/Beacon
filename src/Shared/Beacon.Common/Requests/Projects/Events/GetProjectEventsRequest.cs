using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects.Events;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectEventsRequest : BeaconRequest<GetProjectEventsRequest, ProjectEventDto[]>
{
    public Guid ProjectId { get; set; }
    public DateOnly? MinDate { get; set; }
    public DateOnly? MaxDate { get; set; }
}
