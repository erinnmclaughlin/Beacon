using Beacon.Common.Models;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public sealed class AddProjectContactRequest : IRequest
{
    public required Guid ProjectId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;

    public sealed class Validator : AbstractValidator<AddProjectContactRequest>
    {
        public Validator()
        {
            // TODO: more validation logic
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MinimumLength(3).WithMessage("Contact name must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Contact name cannot exceed 100 characters.");
        }
    }
}
