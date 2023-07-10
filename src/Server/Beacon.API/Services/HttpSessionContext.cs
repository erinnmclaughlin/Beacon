using Beacon.App.Exceptions;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Services;

internal sealed class HttpSessionContext : ISessionContext, ILabContext
{
    public CurrentUser CurrentUser { get; }
    public CurrentLab? CurrentLab { get; }

    CurrentLab ILabContext.CurrentLab => CurrentLab ?? throw new UserNotAllowedException();

    public HttpSessionContext(IHttpContextAccessor httpContextAccessor)
    {
        var principal = httpContextAccessor.HttpContext!.User;

        CurrentUser = CurrentUser.FromClaimsPrincipal(principal);
        CurrentLab = CurrentLab.FromClaimsPrincipal(principal);
    }
}
