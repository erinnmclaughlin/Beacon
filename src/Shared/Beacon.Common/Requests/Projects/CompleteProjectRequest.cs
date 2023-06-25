using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CompleteProjectRequest : IRequest
{
    public required Guid ProjectId { get; set; }
}
