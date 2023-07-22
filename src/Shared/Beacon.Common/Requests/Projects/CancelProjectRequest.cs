using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CancelProjectRequest : BeaconRequest<CancelProjectRequest>, IRequest
{
    public required Guid ProjectId { get; set; }
}
