using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Beacon.API.Domain.Exceptions;
using Beacon.Common.Laboratories.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.App.Features.Laboratories;

public static class UpdateUserMembership
{
    public sealed record Command(Guid LaboratoryId, Guid MemberId, LaboratoryMembershipType MembershipType) : IRequest;

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
            await EnsureLaboratoryExists(request.LaboratoryId, ct);

            var member = await _unitOfWork
                .QueryFor<LaboratoryMembership>(enableChangeTracking: true)
                .FirstOrDefaultAsync(m => m.LaboratoryId == request.LaboratoryId && m.MemberId == request.MemberId, ct)
                ?? throw new LaboratoryMembershipNotFoundException(request.LaboratoryId, request.MemberId);

            await EnsureCurrentUserHasPermission(request, member, ct);

            member.MembershipType = request.MembershipType;
            await _unitOfWork.SaveChangesAsync(ct);
        }

        private async Task EnsureLaboratoryExists(Guid labId, CancellationToken ct)
        {
            var labExists = await _unitOfWork.QueryFor<Laboratory>().AnyAsync(l => l.Id == labId, ct);

            if (!labExists)
                throw new LaboratoryNotFoundException(labId);
        }

        private async Task EnsureCurrentUserHasPermission(Command request, LaboratoryMembership membership, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;

            if (request.MemberId == currentUserId)
                throw new UserNotAllowedException("Users are not allowed to change their own permissions.");

            var currentUser = await _unitOfWork
                .QueryFor<LaboratoryMembership>()
                .Where(m => m.LaboratoryId == request.LaboratoryId && m.MemberId == currentUserId)
                .Select(m => new { m.MembershipType })
                .FirstOrDefaultAsync(ct);

            if (currentUser is null || currentUser.MembershipType is not LaboratoryMembershipType.Admin and not LaboratoryMembershipType.Manager)
                throw new UserNotAllowedException("The current user does not have permission to modify user priveleges for this laboratory.");

            if (currentUser.MembershipType is LaboratoryMembershipType.Admin)
                return;

            if (request.MembershipType is LaboratoryMembershipType.Admin || membership.MembershipType is LaboratoryMembershipType.Admin)
                throw new UserNotAllowedException("Only admins are allowed to grant access to other admins.");
        }

    }
}
