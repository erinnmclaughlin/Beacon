using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CancelProjectRequest : BeaconRequest<CancelProjectRequest>
{
    public required Guid ProjectId { get; set; }
}
