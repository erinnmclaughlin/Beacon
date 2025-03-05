using System.Text.Json.Serialization;
using Beacon.Common.Models;

namespace Beacon.Common.Requests.Laboratories;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetLaboratoryEventsRequest : BeaconRequest<GetLaboratoryEventsRequest, PagedList<LaboratoryEventDto>>, IPaginated
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public List<Guid> ProjectIds { get; set; } = [];
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? MinStart { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? MaxStart { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? MinEnd { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? MaxEnd { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 100;
}
