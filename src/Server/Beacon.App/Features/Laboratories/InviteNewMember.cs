using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Laboratories.Enums;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class InviteNewMember
{
    public sealed record Command : IRequest
    {
        public required string NewMemberEmailAddress { get; init; }
        public required LaboratoryMembershipType MembershipType { get; init; }
        public required Guid LaboratoryId { get; init; }
    }

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.NewMemberEmailAddress)
                .EmailAddress().WithMessage("Email address is invalid.");
        }
    }

    internal sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly ICurrentUser _currentUser;
        private readonly LabInvitationEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(ICurrentUser currentUser, LabInvitationEmailService emailService, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            await EnsureLaboratoryExists(request.LaboratoryId, ct);
            await EnsureInviteeIsNotAlreadyAMember(request.LaboratoryId, request.NewMemberEmailAddress, ct);
            await EnsureCurrentUserCanSendInvites(request.LaboratoryId);

            var invitation = await CreateInvitation(request, ct);
            await _emailService.SendAsync(invitation.Id, ct);
        }

        private async Task<LaboratoryInvitationEmail> CreateInvitation(Command request, CancellationToken ct)
        {
            var now = DateTimeOffset.UtcNow;

            var invitation = new LaboratoryInvitation
            {
                Id = Guid.NewGuid(),
                CreatedOn = now,
                ExpireAfterDays = 10, //TODO: make this configurable
                NewMemberEmailAddress = request.NewMemberEmailAddress,
                MembershipType = request.MembershipType,
                LaboratoryId = request.LaboratoryId,
                CreatedById = _currentUser.UserId
            };

            var emailInvitation = invitation.AddEmailInvitation();

            _unitOfWork.GetRepository<LaboratoryInvitation>().Add(invitation);
            await _unitOfWork.SaveChangesAsync(ct);

            return emailInvitation;
        }

        private async Task EnsureLaboratoryExists(Guid labId, CancellationToken ct)
        {
            var labExists = await _unitOfWork
                .QueryFor<Laboratory>()
                .AnyAsync(l => l.Id == labId, ct);

            if (!labExists)
                throw new LaboratoryNotFoundException(labId);
        }

        private async Task EnsureInviteeIsNotAlreadyAMember(Guid labId, string newMemberEmailAddress, CancellationToken ct)
        {
            var isMember = await _unitOfWork
                .QueryFor<LaboratoryMembership>()
                .AnyAsync(m => m.LaboratoryId == labId && m.Member.EmailAddress == newMemberEmailAddress, ct);

            if (isMember)
            {
                var failure = new ValidationFailure(nameof(Command.NewMemberEmailAddress), $"User with email {newMemberEmailAddress} is already a member of the specified lab.");
                throw new ValidationException(new[] { failure });
            }
        }

        private async Task EnsureCurrentUserCanSendInvites(Guid labId)
        {
            var currentUserId = _currentUser.UserId;

            var membership = await _unitOfWork
                .QueryFor<LaboratoryMembership>()
                .FirstOrDefaultAsync(m => m.MemberId == currentUserId && m.LaboratoryId == labId)
                ?? throw new LaboratoryMembershipRequiredException(labId);

            if (membership.MembershipType is not LaboratoryMembershipType.Admin and not LaboratoryMembershipType.Manager)
                throw new UserNotAllowedException("Only laboratory admins or managers are allowed to invite new members.");
        }
    }
}
