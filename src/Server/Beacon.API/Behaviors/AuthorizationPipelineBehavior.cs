using Beacon.API.Services;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common;
using Beacon.Common.Memberships;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Beacon.API.Behaviors;

public sealed class AuthorizationPipelineBehavior<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ICurrentUser _currentUser;
    private readonly LaboratoryContext _labContext;

    public AuthorizationPipelineBehavior(ICurrentUser currentUser, LaboratoryContext labContext)
    {
        _currentUser = currentUser;
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
        return await _labContext.GetMembershipAsync(_currentUser.UserId, ct) is { } m && types.Contains(m.MembershipType);
    }
}
