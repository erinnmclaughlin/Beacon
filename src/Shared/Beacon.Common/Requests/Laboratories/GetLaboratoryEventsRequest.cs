using Beacon.Common.Models;

namespace Beacon.Common.Requests.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetLaboratoryEventsRequest : BeaconRequest<GetLaboratoryEventsRequest, PagedList<LaboratoryEventDto>>, IPaginated
{
    public List<Guid> ProjectIds { get; set; } = new();
    public DateTime? MinStart { get; set; }
    public DateTime? MaxStart { get; set; }
    public DateTime? MinEnd { get; set; }
    public DateTime? MaxEnd { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}
