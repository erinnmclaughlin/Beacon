using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Projects.Requests;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectsRequest : IRequest<ProjectDto[]>
{
    public required Guid LaboratoryId { get; set; }
}
