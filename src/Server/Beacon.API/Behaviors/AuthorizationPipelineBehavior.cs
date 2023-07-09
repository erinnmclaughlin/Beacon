using Beacon.App.Exceptions;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Services;
using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Beacon.API.Behaviors;

public sealed class AuthorizationPipelineBehavior<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;
    private readonly ICurrentUser _currentUser;

    public AuthorizationPipelineBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers, ICurrentUser currentUser, ILabContext labContext)
    {
        _authorizers = authorizers;
        _currentUser = currentUser;
    }

    public async Task Process(TRequest request, CancellationToken ct)
    {
        if (_currentUser.UserId == Guid.Empty && !AllowsAnonymous())
            throw new UnauthorizedAccessException();

        if (HasMembershipRequirement(out var allowedRoles) && !CurrentUserIsMember(allowedRoles))
            throw new UserNotAllowedException();

        foreach (var authorizer in _authorizers)
        {
            if (!await authorizer.IsAuthorizedAsync(request, ct))
                throw new UserNotAllowedException();
        }
    }

    private static bool AllowsAnonymous()
    {
        return typeof(TRequest).GetCustomAttribute<AllowAnonymousAttribute>() is not null;
    }

    private static bool HasMembershipRequirement(out LaboratoryMembershipType[] types)
    {
        var requirement = typeof(TRequest).GetCustomAttribute<RequireMinimumMembershipAttribute>();
        types = requirement?.AllowedRoles ?? Array.Empty<LaboratoryMembershipType>();
        return requirement is not null;
    }

    private bool CurrentUserIsMember(LaboratoryMembershipType[] types)
    {
        return _currentUser.MembershipType is { } type && types.Contains(type);
    }
}
