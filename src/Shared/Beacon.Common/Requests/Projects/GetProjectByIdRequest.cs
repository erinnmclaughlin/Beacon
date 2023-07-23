using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectByIdRequest : BeaconRequest<GetProjectByIdRequest, ProjectDto>
{
    public Guid ProjectId { get; set; }
}
