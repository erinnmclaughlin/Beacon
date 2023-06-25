using Beacon.Common.Memberships;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Invitations;

[RequireMinimumMembership(LaboratoryMembershipType.Manager)]
public sealed class InviteLabMemberRequest : IRequest
{
    public string NewMemberEmailAddress { get; set; } = string.Empty;
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;

    public sealed class Validator : AbstractValidator<InviteLabMemberRequest>
    {
        public Validator()
        {
            RuleFor(x => x.NewMemberEmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");
        }
    }
}
