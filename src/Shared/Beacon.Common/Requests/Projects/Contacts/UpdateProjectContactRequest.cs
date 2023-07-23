﻿using Beacon.Common.Models;
using Beacon.Common.Validation.Rules;
using FluentValidation;

namespace Beacon.Common.Requests.Projects.Contacts;

[RequireMinimumMembership(LaboratoryMembershipType.Analyst)]
public class UpdateProjectContactRequest : BeaconRequest<UpdateProjectContactRequest>
{
    public Guid ProjectId { get; set; }
    public Guid ContactId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }

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
