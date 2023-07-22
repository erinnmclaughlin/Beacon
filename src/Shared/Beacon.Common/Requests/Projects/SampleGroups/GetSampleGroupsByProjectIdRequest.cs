using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects.SampleGroups;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetSampleGroupsByProjectIdRequest : BeaconRequest<GetSampleGroupsByProjectIdRequest, SampleGroupDto[]>
{
    public Guid ProjectId { get; init; }
}
