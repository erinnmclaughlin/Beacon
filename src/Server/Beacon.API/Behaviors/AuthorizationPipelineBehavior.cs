using Beacon.API.Persistence;
using Beacon.App.Exceptions;
using Beacon.Common;
using Beacon.Common.Memberships;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Beacon.API.Behaviors;

public sealed class AuthorizationPipelineBehavior<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ICurrentUser _currentUser;
    private readonly BeaconDbContext _dbContext;
    private readonly ILabContext _labContext;

    public AuthorizationPipelineBehavior(ICurrentUser currentUser, BeaconDbContext dbContext, ILabContext labContext)
    {
        _currentUser = currentUser;
        _dbContext = dbContext;
        _labContext = labContext;
    }

    public async Task Process(TRequest request, CancellationToken ct)
    {
        if (HasMembershipRequirement(out var allowedRoles) && !await CurrentUserIsMember(allowedRoles, ct))
            throw new UserNotAllowedException();
    }

    private static bool HasMembershipRequirement(out LaboratoryMembershipType[] types)
    {
        var requirement = typeof(TRequest).GetCustomAttribute<RequireMinimumMembershipAttribute>();
        types = requirement?.AllowedRoles ?? Array.Empty<LaboratoryMembershipType>();
        return requirement is not null;
    }

    private async Task<bool> CurrentUserIsMember(LaboratoryMembershipType[] types, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var m = await _dbContext.Memberships
            .Where(x => x.LaboratoryId == _labContext.LaboratoryId && x.MemberId == userId)
            .Select(x => new { x.MembershipType })
            .SingleOrDefaultAsync(ct);

        return m != null && types.Contains(m.MembershipType);
    }
}
