using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectsRequest : BeaconRequest<GetProjectsRequest, ProjectDto[]> { }
