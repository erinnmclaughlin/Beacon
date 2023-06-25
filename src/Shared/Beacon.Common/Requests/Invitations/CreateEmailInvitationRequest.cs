using Beacon.Common.Models;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Requests.Invitations;

[RequireMinimumMembership(LaboratoryMembershipType.Manager)]
public sealed class CreateEmailInvitationRequest : IRequest
{
    public string NewMemberEmailAddress { get; set; } = string.Empty;
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;

    public sealed class Validator : AbstractValidator<CreateEmailInvitationRequest>
    {
        public Validator()
        {
            RuleFor(x => x.NewMemberEmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");
        }
    }
}
