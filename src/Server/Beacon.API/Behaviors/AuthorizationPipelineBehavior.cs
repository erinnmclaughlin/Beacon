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
    private readonly ICurrentUser _currentUser;
    private readonly ILabContext _labContext;

    private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;

    public AuthorizationPipelineBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers, ICurrentUser currentUser, ILabContext labContext)
    {
        _authorizers = authorizers;

        _currentUser = currentUser;
        _labContext = labContext;
    }

    public async Task Process(TRequest request, CancellationToken ct)
    {
        if (HasMembershipRequirement(out var allowedRoles) && !await CurrentUserIsMember(allowedRoles, ct))
            throw new UserNotAllowedException();

        foreach (var authorizer in _authorizers)
        {
            if (!await authorizer.IsAuthorizedAsync(request, ct))
                throw new UserNotAllowedException();
        }
    }

    private static bool HasMembershipRequirement(out LaboratoryMembershipType[] types)
    {
        var requirement = typeof(TRequest).GetCustomAttribute<RequireMinimumMembershipAttribute>();
        types = requirement?.AllowedRoles ?? Array.Empty<LaboratoryMembershipType>();
        return requirement is not null;
    }

    private async Task<bool> CurrentUserIsMember(LaboratoryMembershipType[] types, CancellationToken ct)
    {
        var type = await _labContext.GetMembershipTypeAsync(_currentUser.UserId, ct);
        return type is not null && types.Contains(type.Value);
    }
}
