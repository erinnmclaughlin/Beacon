using Beacon.Common.Memberships;
using MediatR;

namespace Beacon.Common.Projects.Requests;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CancelProjectRequest : LaboratoryRequestBase, IRequest
{
    public required Guid ProjectId { get; set; }
}
