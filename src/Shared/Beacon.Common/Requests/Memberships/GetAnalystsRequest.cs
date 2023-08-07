using Beacon.Common.Models;

namespace Beacon.Common.Requests.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetAnalystsRequest : BeaconRequest<GetAnalystsRequest, LaboratoryMemberDto[]>
{
    public bool IncludeHistoricAnalysts { get; set; } = true;
}
