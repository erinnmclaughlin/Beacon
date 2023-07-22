using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects.SampleGroups;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetSampleGroupsByProjectIdRequest : BeaconRequest<GetSampleGroupsByProjectIdRequest>, IRequest<SampleGroupDto[]>
{
    public required Guid ProjectId { get; init; }
}
