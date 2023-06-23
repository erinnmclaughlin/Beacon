using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Memberships;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Memberships;

public sealed class UpdateMembership : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut<UpdateMembershipRequest>("memberships").WithTags(EndpointTags.Memberships);
    }

    internal sealed class Handler : IRequestHandler<UpdateMembershipRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task Handle(UpdateMembershipRequest request, CancellationToken ct)
        {
            var member = await _dbContext.Memberships
                .SingleAsync(m => m.LaboratoryId == request.LaboratoryId && m.MemberId == request.MemberId, ct);

            await EnsureCurrentUserHasPermission(request, member, ct);

            member.MembershipType = request.MembershipType;
            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task EnsureCurrentUserHasPermission(UpdateMembershipRequest request, Membership memberToUpdate, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;

            if (request.MemberId == currentUserId)
                throw new UserNotAllowedException("Users are not allowed to change their own permissions.");

            var membership = await _dbContext.Memberships
                .AsNoTracking()
                .SingleAsync(m => m.MemberId == currentUserId && m.LaboratoryId == request.LaboratoryId, ct);

            if (membership.MembershipType is LaboratoryMembershipType.Admin)
                return;

            if (request.MembershipType is LaboratoryMembershipType.Admin || memberToUpdate.MembershipType is LaboratoryMembershipType.Admin)
                throw new UserNotAllowedException("Only admins are allowed to grant access to other admins.");

            if (membership.MembershipType is not LaboratoryMembershipType.Manager)
                throw new UserNotAllowedException("The current user does not have permission to modify membership types for this laboratory.");
        }
    }
}
