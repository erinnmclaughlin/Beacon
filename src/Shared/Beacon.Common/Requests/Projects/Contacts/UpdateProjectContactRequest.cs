using Beacon.Common.Models;
using Beacon.Common.Validation.Rules;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Requests.Projects.Contacts;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public class UpdateProjectContactRequest : IRequest
{
    public required Guid ProjectId { get; set; }
    public required Guid ContactId { get; set; }
    public required string Name { get; set; }
    public required string? PhoneNumber { get; set; }
    public required string? EmailAddress { get; set; }

    public sealed class Validator : AbstractValidator<UpdateProjectContactRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).IsValidContactName();
            RuleFor(x => x.EmailAddress).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.EmailAddress)).WithMessage("Email address is invalid.");
            RuleFor(x => x.PhoneNumber).IsValidPhoneNumber().When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
