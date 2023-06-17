using Beacon.Common.Laboratories;

namespace Beacon.App.Services;

public interface ISessionManager
{
    Guid UserId { get; }
    Guid LabId { get; }
    LaboratoryMembershipType MembershipType { get; }

    Task SignInAsync(Guid userId);
    Task SignOutAsync();

    Task SetCurrentLabAsync(Guid labId, LaboratoryMembershipType membershipType);
    Task ClearCurrentLabAsync();
}
