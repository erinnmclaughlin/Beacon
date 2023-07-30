using Beacon.Common.Models;

namespace Beacon.Common.Requests.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetMembershipsRequest : BeaconRequest<GetMembershipsRequest, LaboratoryMemberDto[]> 
{
    
    public LaboratoryMembershipType? MinimumRole { get; set; }
}
