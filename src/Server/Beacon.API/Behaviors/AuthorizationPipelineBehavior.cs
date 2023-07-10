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
    private readonly ISessionContext _context;

    public AuthorizationPipelineBehavior(ISessionContext context)
    {
        _context = context;
    }

    public Task Process(TRequest request, CancellationToken ct)
    {
        if (_context.UserId == Guid.Empty && !AllowsAnonymous())
            throw new UnauthorizedAccessException();

        if (HasMembershipRequirement(out var allowedRoles) && !CurrentUserIsMember(allowedRoles))
            throw new UserNotAllowedException();

        return Task.CompletedTask;
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
