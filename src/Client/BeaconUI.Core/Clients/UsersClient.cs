using Beacon.Common;
using ErrorOr;

namespace BeaconUI.Core.Clients;

public sealed class UsersClient : ApiClientBase
{
    public UsersClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public Task<ErrorOr<UserDto>> GetById(Guid userId, CancellationToken ct = default)
    {
        return GetAsync<UserDto>($"api/users/{userId}", ct);
    }
}
