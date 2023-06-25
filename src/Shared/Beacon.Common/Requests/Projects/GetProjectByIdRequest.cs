using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectByIdRequest : IRequest<ProjectDto?>
{
    public required Guid ProjectId { get; set; }
}
