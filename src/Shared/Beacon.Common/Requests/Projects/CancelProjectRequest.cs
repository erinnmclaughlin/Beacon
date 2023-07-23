using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CancelProjectRequest : BeaconRequest<CancelProjectRequest>
{
    public Guid ProjectId { get; set; }
}
