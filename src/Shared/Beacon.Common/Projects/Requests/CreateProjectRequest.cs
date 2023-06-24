using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Projects.Requests;

public class CreateProjectRequest : IRequest
{
    public required Guid LaboratoryId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    public class Validator : AbstractValidator<CreateProjectRequest>
    {
        public Validator()
        {
            RuleFor(x => x.CustomerCode).IsValidCustomerCode();
            RuleFor(x => x.CustomerName).IsValidCustomerName();
        }
    }
}
