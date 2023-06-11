using Beacon.App.Entities;

namespace Beacon.App.Services;

public interface ICurrentUser
{
    Guid UserId { get; }
    Task<User> GetCurrentUserAsync(CancellationToken ct = default);
}
