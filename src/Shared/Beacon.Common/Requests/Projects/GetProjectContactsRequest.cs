using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectContactsRequest : IRequest<ProjectContactDto[]>
{
    public required Guid ProjectId { get; set; }
}
