using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectsRequest : IRequest<ProjectDto[]> { }
