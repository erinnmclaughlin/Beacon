using Beacon.Common.Models;
using Beacon.Common.Validation.Rules;
using FluentValidation;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class CreateProjectRequest : BeaconRequest<CreateProjectRequest, ProjectCode>
{
    public string Application { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public Guid? LeadAnalystId { get; set; }

    public class Validator : AbstractValidator<CreateProjectRequest>
    {
        public Validator()
        {
            RuleFor(x => x.CustomerCode).IsValidCustomerCode();
            RuleFor(x => x.CustomerName).IsValidCustomerName();
        }
    }
}
