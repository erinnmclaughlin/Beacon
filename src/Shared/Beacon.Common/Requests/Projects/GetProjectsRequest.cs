using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectsRequest : BeaconRequest<GetProjectsRequest, PagedList<ProjectDto>>, IPaginated
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public List<ProjectStatus> IncludedStatuses { get; set; } = [];
    public List<Guid> LeadAnalystIds { get; set; } = [];
}
