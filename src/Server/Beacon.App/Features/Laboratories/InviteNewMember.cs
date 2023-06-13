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
        private readonly LabInvitationEmailService _emailService;
        private readonly ISessionManager _sessionManager;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(LabInvitationEmailService emailService, ISessionManager sessionManger, IUnitOfWork unitOfWork)
        {
            _emailService = emailService;
            _sessionManager = sessionManger;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            EnsureCurrentUserIsAllowed(request);

            await EnsureInviteeIsNotAlreadyAMember(request.NewMemberEmailAddress, ct);

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
                LaboratoryId = _sessionManager.LabId,
                CreatedById = _sessionManager.UserId
            };

            var emailInvitation = invitation.AddEmailInvitation();

            _unitOfWork.GetRepository<LaboratoryInvitation>().Add(invitation);
            await _unitOfWork.SaveChangesAsync(ct);

            return emailInvitation;
        }

        private void EnsureCurrentUserIsAllowed(Command request)
        {
            if (_sessionManager.MembershipType is LaboratoryMembershipType.Admin)
                return;

            if (request.MembershipType is LaboratoryMembershipType.Admin)
                throw new UserNotAllowedException("Only laboratory admins are allowed to invite new admins.");

            if (_sessionManager.MembershipType is not LaboratoryMembershipType.Manager)
                throw new UserNotAllowedException("Only laboratory admins or managers are allowed to invite new members.");
        }

        private async Task EnsureInviteeIsNotAlreadyAMember(string newMemberEmailAddress, CancellationToken ct)
        {
            var labId = _sessionManager.LabId;

            var isMember = await _unitOfWork
                .QueryFor<LaboratoryMembership>()
                .AnyAsync(m => m.LaboratoryId == labId && m.Member.EmailAddress == newMemberEmailAddress, ct);

            if (isMember)
            {
                var failure = new ValidationFailure(nameof(Command.NewMemberEmailAddress), $"User with email {newMemberEmailAddress} is already a member of the specified lab.");
                throw new ValidationException(new[] { failure });
            }
        }
    }
}
