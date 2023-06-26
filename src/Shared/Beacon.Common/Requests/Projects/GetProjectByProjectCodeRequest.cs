using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectByProjectCodeRequest : IRequest<ProjectDto?>
{
    public required ProjectCode ProjectCode { get; set; }
}