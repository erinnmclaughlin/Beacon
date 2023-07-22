using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectByProjectCodeRequest : BeaconRequest<GetProjectByProjectCodeRequest, ProjectDto>
{
    public required ProjectCode ProjectCode { get; set; }
}