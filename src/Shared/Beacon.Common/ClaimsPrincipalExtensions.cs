using System.Security.Claims;

namespace Beacon.Common;

public static class ClaimsPrincipalExtensions
{
    public static T? FindEnumValue<T>(this ClaimsPrincipal user, string type) where T : struct
    {
        var value = user.FindFirst(type)?.Value;
        return Enum.TryParse<T>(value, out var result) ? result : null;
    }

    public static Guid FindGuidValue(this ClaimsPrincipal user, string type)
    {
        return Guid.TryParse(user.FindFirst(type)?.Value, out var id) ? id : Guid.Empty;
    }

}