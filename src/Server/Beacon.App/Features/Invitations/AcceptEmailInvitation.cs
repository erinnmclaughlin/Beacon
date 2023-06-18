using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Memberships;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Invitations;

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
            var currentUserId = _currentUser.UserId;

            var currentUser = await _unitOfWork
                .QueryFor<User>()
                .FirstAsync(u => u.Id == currentUserId, ct);

            var emailInvite = await FindInvitation(request, ct);

            emailInvite.Accept(currentUser, DateTimeOffset.UtcNow);

            await AddLabMember(currentUser.Id, emailInvite.LaboratoryInvitation.LaboratoryId, emailInvite.LaboratoryInvitation.MembershipType, ct);

            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task<InvitationEmail> FindInvitation(Command request, CancellationToken ct)
        {
            return await _unitOfWork
                .QueryFor<InvitationEmail>(enableChangeTracking: true)
                .Include(i => i.LaboratoryInvitation)
                .FirstOrDefaultAsync(i => i.Id == request.EmailId && i.LaboratoryInvitationId == request.InviteId, ct)
                ?? throw new EmailInvitationNotFoundException(request.EmailId, request.InviteId);
        }

        private async Task AddLabMember(Guid acceptingUserId, Guid labId, LaboratoryMembershipType membershipType, CancellationToken ct)
        {
            var alreadyAMember = await _unitOfWork
                .QueryFor<Membership>()
                .AnyAsync(m => m.LaboratoryId == labId && m.MemberId == acceptingUserId, ct);

            if (alreadyAMember)
                return;

            _unitOfWork
                .GetRepository<Membership>()
                .Add(new Membership
                {
                    LaboratoryId = labId,
                    MemberId = acceptingUserId,
                    MembershipType = membershipType
                });
        }
    }
}
