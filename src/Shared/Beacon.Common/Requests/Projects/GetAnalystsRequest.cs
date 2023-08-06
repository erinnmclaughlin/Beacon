using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

public sealed class GetAnalystsRequest : BeaconRequest<GetAnalystsRequest, LaboratoryMemberDto[]>
{
    public bool IncludeHistoricAnalysts { get; set; } = true;
}
