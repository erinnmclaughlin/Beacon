using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.GetCurrentUser;
using ErrorOr;

namespace BeaconUI.Core.Features.Auth;

public class GetCurrentUserRequestHandler : IApiRequestHandler<GetCurrentUserRequest, UserDto>
{
    private readonly HttpClient _httpClient;

    public GetCurrentUserRequestHandler(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ErrorOr<UserDto>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("api/users/current", cancellationToken);
        return await response.ToErrorOrResult<UserDto>(cancellationToken);
    }
}
