using System.Security.Claims;

namespace Beacon.Common.Services;

public class CurrentUser
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }

    public Claim[] GetClaims() => new[]
    {
        new Claim(BeaconClaimTypes.UserId, Id.ToString()),
        new Claim(BeaconClaimTypes.DisplayName, DisplayName)
    };

    public static CurrentUser FromClaimsPrincipal(ClaimsPrincipal user) => new()
    {
        Id = user.FindGuidValue(BeaconClaimTypes.UserId),
        DisplayName = user.FindFirst(BeaconClaimTypes.DisplayName)?.Value ?? ""
    };
}
