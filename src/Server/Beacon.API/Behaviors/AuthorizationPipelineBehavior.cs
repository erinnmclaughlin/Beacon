using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Services;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Beacon.API.Behaviors;

public sealed class AuthorizationPipelineBehavior<TRequest, TResponse>(ISessionContext context) : IPipelineBehavior<TRequest, ErrorOr<TResponse>> where TRequest : notnull
{
    private readonly ISessionContext _context = context;

    public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken ct)
    {
        if (_context.UserId == Guid.Empty && !AllowsAnonymous())
            return BeaconError.Unauthorized();

        if (HasMembershipRequirement(out var allowedRoles) && !CurrentUserIsMember(allowedRoles))
            return BeaconError.Forbid("The current user does not have permission to perform that action.");

        return await next();
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
        return _context.CurrentLab?.MembershipType is { } type && types.Contains(type);
    }
}
