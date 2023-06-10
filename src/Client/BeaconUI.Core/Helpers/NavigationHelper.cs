namespace BeaconUI.Core.Helpers;

public static class NavigationHelper
{
    public static string GetLabDetailsHref(Guid labId) => $"laboratories/{labId}";
    public static string GetLabMembersPageHref(Guid labId) => $"laboratories/{labId}/members";
}
