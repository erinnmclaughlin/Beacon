using Beacon.Common.Models;
using System.Security.Claims;

namespace Beacon.Common.Services;

public class CurrentLab
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public Claim[] GetClaims() => new[]
    {
        new Claim(BeaconClaimTypes.LabId, Id.ToString()),
        new Claim(BeaconClaimTypes.LabName, Name.ToString()),
        new Claim(BeaconClaimTypes.MembershipType, MembershipType.ToString())
    };

    public static CurrentLab? FromClaimsPrincipal(ClaimsPrincipal user)
    {
        var id = user.FindGuidValue(BeaconClaimTypes.LabId);
        var membershipType = user.FindEnumValue<LaboratoryMembershipType>(BeaconClaimTypes.MembershipType);

        if (id == default || membershipType == null)
            return null;

        return new CurrentLab
        {
            Id = id,
            Name = user.FindFirst(BeaconClaimTypes.LabName)?.Value ?? "",
            MembershipType = membershipType.Value
        };
    }
}