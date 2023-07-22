using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects.Contacts;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class DeleteProjectContactRequest : BeaconRequest<DeleteProjectContactRequest>
{
    public required Guid ProjectId { get; set; }
    public required Guid ContactId { get; set; }
}
