using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Events;
using Beacon.Common.Auth.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;
using MediatR;
using System.Net.Http.Json;

namespace BeaconUI.Core.Auth.RequestHandlers;

public class LoginRequestHandler : IApiRequestHandler<LoginRequest, UserDto>
{
    private readonly HttpClient _httpClient;
    private readonly IPublisher _publisher;

    public LoginRequestHandler(HttpClient httpClient, IPublisher publisher)
    {
        _httpClient = httpClient;
        _publisher = publisher;
    }

    public async Task<ErrorOr<UserDto>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        var result = await response.ToErrorOrResult<UserDto>(cancellationToken);

        if (!result.IsError)
            await _publisher.Publish(new LoginEvent(result.Value), cancellationToken);

        return result;
    }
}
