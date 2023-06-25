using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.Common;
using Beacon.Common.Memberships;
using Beacon.Common.Requests.Memberships;
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
        private readonly ILabContext _labContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext, ILabContext labContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task Handle(UpdateMembershipRequest request, CancellationToken ct)
        {
            var labId = _labContext.LaboratoryId;
            var member = await _dbContext.Memberships
                .SingleAsync(m => m.LaboratoryId == labId && m.MemberId == request.MemberId, ct);

            await EnsureCurrentUserHasPermission(request, member, ct);

            member.MembershipType = request.MembershipType;
            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task EnsureCurrentUserHasPermission(UpdateMembershipRequest request, Membership memberToUpdate, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;
            var labId = _labContext.LaboratoryId;

            if (request.MemberId == currentUserId)
                throw new UserNotAllowedException("Users are not allowed to change their own permissions.");

            var membership = await _dbContext.Memberships
                .AsNoTracking()
                .SingleAsync(m => m.MemberId == currentUserId && m.LaboratoryId == labId, ct);

            if (membership.MembershipType is LaboratoryMembershipType.Admin)
                return;

            if (request.MembershipType is LaboratoryMembershipType.Admin || memberToUpdate.MembershipType is LaboratoryMembershipType.Admin)
                throw new UserNotAllowedException("Only admins are allowed to grant access to other admins.");
        }
    }
}
