using Beacon.Common.Validation.Rules;
using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class CreateLaboratoryRequest
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
