using Beacon.Common.Models;
using System.Security.Claims;

namespace Beacon.Common.Services;

public interface ILabContext
{
    CurrentUser CurrentUser { get; }
    CurrentLab CurrentLab { get; }
    public LaboratoryMembershipType MembershipType => CurrentLab.MembershipType;
}

public class LabContext : ILabContext
{
    public required CurrentUser CurrentUser { get; init; }
    public required CurrentLab CurrentLab { get; init; }

    public static LabContext? FromClaimsPrincipal(ClaimsPrincipal principal) => FromSessionContext(SessionContext.FromClaimsPrincipal(principal));

    public static LabContext? FromSessionContext(SessionContext? context) => context?.CurrentLab == null ? null : new() 
    { 
        CurrentLab = context.CurrentLab, 
        CurrentUser = context.CurrentUser 
    };
}