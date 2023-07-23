using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class UpdateLeadAnalystRequest : BeaconRequest<UpdateLeadAnalystRequest>
{
    public Guid? AnalystId { get; set; }
    public Guid ProjectId { get; set; }
}
