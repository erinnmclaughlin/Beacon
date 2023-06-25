using Beacon.Common;
using Beacon.Common.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Services;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(BeaconClaimTypes.UserId);
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }
}
