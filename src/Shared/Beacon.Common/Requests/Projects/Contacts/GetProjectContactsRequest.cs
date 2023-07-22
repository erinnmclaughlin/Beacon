using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects.Contacts;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectContactsRequest : BeaconRequest<GetProjectContactsRequest, ProjectContactDto[]>
{
    public required Guid ProjectId { get; set; }
}
