using FluentValidation;

namespace Beacon.Common.Laboratories.Requests;

public class CreateLaboratoryRequest : IApiRequest<LaboratoryDto>
{
    public string Name { get; set; } = string.Empty;

    public class Validator : AbstractValidator<CreateLaboratoryRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }
}
