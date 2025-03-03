using Beacon.Common.Models;
using System.Security.Claims;

namespace Beacon.Common.Services;

public class CurrentLab
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public Claim[] GetClaims() => [
        new(BeaconClaimTypes.LabId, Id.ToString()),
        new(BeaconClaimTypes.LabName, Name),
        new(BeaconClaimTypes.MembershipType, MembershipType.ToString())
    ];

    public static CurrentLab? FromClaimsPrincipal(ClaimsPrincipal? user)
    {
        if (user == null)
            return null;

        var id = user.FindGuidValue(BeaconClaimTypes.LabId);
        var membershipType = user.FindEnumValue<LaboratoryMembershipType>(BeaconClaimTypes.MembershipType);

        if (id == Guid.Empty || membershipType == null)
            return null;

        return new CurrentLab
        {
            Id = id,
            Name = user.FindFirst(BeaconClaimTypes.LabName)?.Value ?? "",
            MembershipType = membershipType.Value
        };
    }
}