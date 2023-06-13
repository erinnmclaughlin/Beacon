using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Laboratories.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Features.Laboratories;

public static class UpdateUserMembership
{
    public sealed record Command(Guid MemberId, LaboratoryMembershipType MembershipType) : IRequest;

    internal sealed class CommandHandler : IRequestHandler<Command>
    {
        private readonly ISessionManager _sessionManager;
        private readonly IUnitOfWork _unitOfWork;

        public CommandHandler(ISessionManager sessionManager, IUnitOfWork unitOfWork)
        {
            _sessionManager = sessionManager;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(Command request, CancellationToken ct)
        {
            var labId = _sessionManager.LabId;

            var member = await _unitOfWork
                .QueryFor<LaboratoryMembership>(enableChangeTracking: true)
                .FirstOrDefaultAsync(m => m.LaboratoryId == labId && m.MemberId == request.MemberId, ct)
                ?? throw new LaboratoryMembershipNotFoundException(labId, request.MemberId);

            EnsureCurrentUserHasPermission(request, member);

            member.MembershipType = request.MembershipType;
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private void EnsureCurrentUserHasPermission(Command request, LaboratoryMembership membership)
        {
            var currentUserId = _sessionManager.UserId;

            if (request.MemberId == currentUserId)
                throw new UserNotAllowedException("Users are not allowed to change their own permissions.");

            if (_sessionManager.MembershipType is LaboratoryMembershipType.Admin)
                return;

            if (_sessionManager.MembershipType is not LaboratoryMembershipType.Manager)
                throw new UserNotAllowedException("The current user does not have permission to modify user priveleges for this laboratory.");

            if (request.MembershipType is LaboratoryMembershipType.Admin || membership.MembershipType is LaboratoryMembershipType.Admin)
                throw new UserNotAllowedException("Only admins are allowed to grant access to other admins.");
        }
    }
}
