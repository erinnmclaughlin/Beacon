using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectByProjectCodeRequest : BeaconRequest<GetProjectByProjectCodeRequest, ProjectDto>
{
    public ProjectCode ProjectCode { get; set; } = default!;
}