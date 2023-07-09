using System.Security.Claims;

namespace Beacon.Common.Models;

public sealed record SessionContext
{
    public required CurrentUserDto CurrentUser { get; init; }
    public required CurrentLabDto? CurrentLab { get; init; }

    public static SessionContext? FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated is not true)
            return null;

        return new SessionContext
        {
            CurrentLab = principal.GetCurrentLab(),
            CurrentUser = principal.GetCurrentUser()
        };
    }

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaims(new[]
        {
            new Claim(BeaconClaimTypes.UserId, CurrentUser.Id.ToString()),
            new Claim(BeaconClaimTypes.DisplayName, CurrentUser.DisplayName)
        });

        if (CurrentLab != null)
        {
            identity.AddClaims(new[]
            {
                new Claim(BeaconClaimTypes.LabId, CurrentLab.Id.ToString()),
                new Claim(BeaconClaimTypes.LabName, CurrentLab.Name.ToString()),
                new Claim(BeaconClaimTypes.MembershipType, CurrentLab.MembershipType.ToString())
            });
        }

        return new ClaimsPrincipal(identity);
    }
}

public sealed record CurrentUserDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
}

public sealed record CurrentLabDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }
}