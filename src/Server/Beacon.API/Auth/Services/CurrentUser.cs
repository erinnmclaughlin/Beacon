using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Auth.Services;

public interface ICurrentUser
{
    Guid UserId { get; }
    ClaimsPrincipal User { get; }
}

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
            var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());
}
