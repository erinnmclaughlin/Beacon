using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CompleteProjectRequest : BeaconRequest<CompleteProjectRequest>
{
    public Guid ProjectId { get; set; }
}
