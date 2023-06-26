using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects.Contacts;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class DeleteProjectContactRequest : IRequest
{
    public required Guid ProjectId { get; set; }
    public required Guid ContactId { get; set; }
}
