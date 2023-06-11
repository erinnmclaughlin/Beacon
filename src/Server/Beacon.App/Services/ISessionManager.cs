using Beacon.Common.Laboratories.Enums;

namespace Beacon.App.Services;

public interface ISessionManager
{
    Guid UserId { get; }
    Guid LabId { get; }
    LaboratoryMembershipType? MembershipType { get; }

    Task SignInAsync(Guid userId);
    Task SignOutAsync();

    void SetCurrentLab(Guid labId, LaboratoryMembershipType membershipType);
    void ClearCurrentLab();
}
