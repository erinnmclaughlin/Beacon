using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Projects.Requests;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectsRequest : LaboratoryRequestBase, IRequest<ProjectDto[]>
{
}
