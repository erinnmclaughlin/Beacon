using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects.Contacts;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class DeleteProjectContactRequest : BeaconRequest<DeleteProjectContactRequest>
{
    public Guid ProjectId { get; set; }
    public Guid ContactId { get; set; }
}
