using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects.Contacts;

[RequireMinimumMembership(LaboratoryMembershipType.Member)]
public sealed class GetProjectContactsRequest : BeaconRequest<GetProjectContactsRequest>, IRequest<ProjectContactDto[]>
{
    public required Guid ProjectId { get; set; }
}
