using Beacon.API.Persistence;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common;
using Beacon.Common.Memberships;
using FluentValidation;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Beacon.API.Behaviors;

public sealed class AuthorizationPipelineBehavior<TRequest> : IRequestPreProcessor<TRequest> where TRequest : LaboratoryRequestBase
{
    private readonly ICurrentUser _currentUser;
    private readonly BeaconDbContext _dbContext;

    public AuthorizationPipelineBehavior(ICurrentUser currentUser, BeaconDbContext dbContext)
    {
        _currentUser = currentUser;
        _dbContext = dbContext;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (typeof(TRequest).GetCustomAttribute<RequireMinimumMembershipAttribute>() is not { } requirement)
            return;

        var membershipType = await GetCurrentUserMembershipType(request.LaboratoryId, cancellationToken);

        if (membershipType is null || !requirement.AllowedRoles.Contains(membershipType.Value))
            throw new UserNotAllowedException();
    }

    private async Task<LaboratoryMembershipType?> GetCurrentUserMembershipType(Guid labId, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var membership = await _dbContext.Memberships
            .Where(m => m.LaboratoryId == labId && m.MemberId == userId)
            .Select(m => new { m.MembershipType })
            .SingleOrDefaultAsync(ct);

        return membership?.MembershipType;
    }
}
