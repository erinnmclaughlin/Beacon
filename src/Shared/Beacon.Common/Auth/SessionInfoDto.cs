using Beacon.Common.Memberships;
using System.Security.Claims;

namespace Beacon.Common.Auth;

public sealed record SessionInfoDto(SessionInfoDto.UserDto CurrentUser, SessionInfoDto.LabDto? CurrentLab)
{
    public sealed record UserDto(Guid Id, string DisplayName);
    public sealed record LabDto(Guid Id, string Name, LaboratoryMembershipType MembershipType);

    public ClaimsPrincipal ToClaimsPrincipal()
    {
        var identity = new ClaimsIdentity("AuthCookie");

        identity.AddClaims(new[]
        {
            new Claim(BeaconClaimTypes.UserId, CurrentUser.Id.ToString()),
            new Claim(BeaconClaimTypes.DisplayName, CurrentUser.DisplayName)
        });

        if (CurrentLab is { } lab)
        {
            identity.AddClaims(new[]
            {
                new Claim(BeaconClaimTypes.LabId, lab.Id.ToString()),
                new Claim(BeaconClaimTypes.LabName, lab.Name),
                new Claim(BeaconClaimTypes.MembershipType, lab.MembershipType.ToString())
            });
        }

        return new ClaimsPrincipal(identity);
    }

    public static SessionInfoDto? FromClaimsPrincipal(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated is not true)
            return null;

        var userId = Guid.Parse(principal.FindFirst(BeaconClaimTypes.UserId)?.Value ?? "");
        var displayName = principal.FindFirst(BeaconClaimTypes.DisplayName)?.Value ?? "";

        var sessionInfo = new SessionInfoDto(new UserDto(userId, displayName), null);

        if (Guid.TryParse(principal.FindFirst(BeaconClaimTypes.LabId)?.Value, out var labId))
        {
            var labName = principal.FindFirst(BeaconClaimTypes.LabName)?.Value ?? "";
            var membershipType = Enum.Parse<LaboratoryMembershipType>(principal.FindFirst(BeaconClaimTypes.MembershipType)?.Value ?? "");
            sessionInfo = sessionInfo with { CurrentLab = new(labId, labName, membershipType) };
        }

        return sessionInfo;
    }
}
