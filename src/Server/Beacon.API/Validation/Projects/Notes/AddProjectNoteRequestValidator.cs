using Beacon.Common.Requests.Projects.Notes;
using FluentValidation;

namespace Beacon.API.Validation.Projects.Notes;

internal sealed class AddProjectNoteRequestValidator : AbstractValidator<AddProjectNoteRequest>
{
    public AddProjectNoteRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .MaximumLength(1000);
    }
} 