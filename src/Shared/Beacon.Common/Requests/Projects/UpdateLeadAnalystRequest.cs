using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public class UpdateLeadAnalystRequest : IRequest
{
    public Guid? AnalystId { get; set; }
    public required Guid ProjectId { get; set; }
}
