using Beacon.Common.Models;
using Beacon.Common.Services;
using FluentValidation;
using MediatR;

namespace Beacon.Common.Requests.Invitations;

[RequireMinimumMembership(LaboratoryMembershipType.Manager)]
public sealed class CreateEmailInvitationRequest : IRequest
{
    public string NewMemberEmailAddress { get; set; } = string.Empty;
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;

    public sealed class Authorizer : IAuthorizer<CreateEmailInvitationRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILabContext _labContext;

        public Authorizer(ICurrentUser currentUser, ILabContext labContext)
        {
            _currentUser = currentUser;
            _labContext = labContext;
        }

        public async Task<bool> IsAuthorizedAsync(CreateEmailInvitationRequest request, CancellationToken ct = default)
        {
            // only admins can invite other admins:
            return request.MembershipType is not LaboratoryMembershipType.Admin ||
                await _labContext.GetMembershipTypeAsync(_currentUser.UserId, ct) is LaboratoryMembershipType.Admin;
        }
    }

    public sealed class Validator : AbstractValidator<CreateEmailInvitationRequest>
    {
        public Validator()
        {
            RuleFor(x => x.NewMemberEmailAddress)
                .EmailAddress().WithMessage("Invalid email address.");
        }
    }
}
