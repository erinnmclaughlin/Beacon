using Beacon.Common.Memberships;
using Beacon.Common.Projects;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectsRequest : IRequest<ProjectDto[]> { }
