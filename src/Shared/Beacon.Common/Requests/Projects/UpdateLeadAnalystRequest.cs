using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class UpdateLeadAnalystRequest : BeaconRequest<UpdateLeadAnalystRequest>, IRequest
{
    public Guid? AnalystId { get; set; }
    public required Guid ProjectId { get; set; }
}
