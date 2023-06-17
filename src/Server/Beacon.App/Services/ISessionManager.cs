using Beacon.Common.Laboratories;

namespace Beacon.App.Services;

public interface ISessionManager : ICurrentUser, ICurrentLab
{
    Task SignInAsync(Guid userId);
    Task SignOutAsync();

    Task SetCurrentLabAsync(Guid labId, LaboratoryMembershipType membershipType);
    Task ClearCurrentLabAsync();
}
