namespace Beacon.App.Services;

public interface ISessionManager
{
    Task SignInAsync(Guid userId);
    Task SignOutAsync();
}
