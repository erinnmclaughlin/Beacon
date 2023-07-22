namespace Beacon.Common.Services;

public interface ISessionContext
{
    CurrentUser CurrentUser { get; }
    CurrentLab? CurrentLab { get; }

    public Guid UserId => CurrentUser.Id;
}
