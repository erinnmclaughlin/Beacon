using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Laboratories.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class AcceptEmailInvitation
{
    public sealed record Command(Guid InviteId, Guid EmailId) : IRequest;

    internal sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            var currentUser = await _currentUser.GetCurrentUserAsync(ct);

            var invitation = await FindInvitation(request, ct);

            invitation.Accept(currentUser);

            await AddLabMember(currentUser.Id, invitation.LaboratoryId, invitation.MembershipType, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<LaboratoryInvitation> FindInvitation(Command request, CancellationToken ct)
        {
            var invitation = await _unitOfWork
                .QueryFor<LaboratoryInvitation>(enableChangeTracking: true)
                .Include(i => i.EmailInvitations)
                .AsSingleQuery()
                .FirstOrDefaultAsync(i => i.Id == request.InviteId, ct);

            if (invitation?.EmailInvitations.FirstOrDefault(i => i.Id == request.EmailId) is null)
                throw new EmailInvitationNotFoundException(request.EmailId, request.InviteId);

            return invitation;
        }

        private async Task AddLabMember(Guid acceptingUserId, Guid labId, LaboratoryMembershipType membershipType, CancellationToken ct)
        {
            var alreadyAMember = await _unitOfWork
                .QueryFor<LaboratoryMembership>()
                .AnyAsync(m => m.LaboratoryId == labId && m.MemberId == acceptingUserId, ct);

            if (alreadyAMember)
                return;

            _unitOfWork
                .GetRepository<LaboratoryMembership>()
                .Add(new LaboratoryMembership
                {
                    LaboratoryId = labId,
                    MemberId = acceptingUserId,
                    MembershipType = membershipType
                });
        }
    }

}
