using Beacon.Common;
using Beacon.Common.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Services;

internal sealed class CurrentUser : ICurrentUser
{
    public Guid UserId { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var idValue = httpContextAccessor.HttpContext?.User.FindFirstValue(BeaconClaimTypes.UserId);
        UserId = Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
    }
}
