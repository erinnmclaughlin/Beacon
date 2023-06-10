using Beacon.API.Domain.Entities;
using System.Security.Claims;

namespace Beacon.API.App.Services;

public interface ICurrentUser
{
    Guid UserId { get; }

    Task<User> GetCurrentUserAsync(CancellationToken ct = default);
}
