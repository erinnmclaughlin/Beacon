using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Laboratories;

public class CreateLaboratoryRequest : IRequest
{
    public string LaboratoryName { get; set; } = string.Empty;

    public class Validator : AbstractValidator<CreateLaboratoryRequest>
    {
        public Validator()
        {
            RuleFor(x => x.LaboratoryName).IsValidLaboratoryName();
        }
    }
}
