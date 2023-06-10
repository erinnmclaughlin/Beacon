using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Beacon.API.Domain.Exceptions;
using Beacon.Common.Laboratories.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Laboratories;

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
            var membershipsRepository = _unitOfWork.GetRepository<LaboratoryMembership>();

            var alreadyAMember = await membershipsRepository
                .AsQueryable()
                .AnyAsync(m => m.LaboratoryId == labId && m.MemberId == acceptingUserId, ct);

            if (alreadyAMember)
                return;

            membershipsRepository.Add(new LaboratoryMembership
            {
                LaboratoryId = labId,
                MemberId = acceptingUserId,
                MembershipType = membershipType
            });
        }
    }

}
