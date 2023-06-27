using Beacon.Common.Models;
using Beacon.Common.Services;
using MediatR;

namespace Beacon.Common.Requests.Memberships;

[RequireMinimumMembership(LaboratoryMembershipType.Manager)]
public sealed class UpdateMembershipRequest : IRequest
{
    public required Guid MemberId { get; set; }
    public LaboratoryMembershipType MembershipType { get; set; } = LaboratoryMembershipType.Member;

    public sealed class Authorizer : IAuthorizer<UpdateMembershipRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ILabContext _labContext;

        public Authorizer(ICurrentUser currentUser, ILabContext labContext)
        {
            _currentUser = currentUser;
            _labContext = labContext;
        }

        public async Task<bool> IsAuthorizedAsync(UpdateMembershipRequest request, CancellationToken ct = default)
        {
            if (_currentUser.UserId == request.MemberId)
                return false;

            var currentUserMembershipType = await _labContext.GetMembershipTypeAsync(_currentUser.UserId, ct);

            return currentUserMembershipType is LaboratoryMembershipType.Admin || (
                currentUserMembershipType is LaboratoryMembershipType.Manager &&
                request.MembershipType is not LaboratoryMembershipType.Admin
                && await _labContext.GetMembershipTypeAsync(request.MemberId, ct) is not LaboratoryMembershipType.Admin);
        }
    }
}
