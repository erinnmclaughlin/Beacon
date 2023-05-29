using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Register;
using BeaconUI.Core.Events;
using ErrorOr;
using MediatR;
using System.Net.Http.Json;

namespace BeaconUI.Core.Features.Auth;

public class RegisterRequestHandler : IApiRequestHandler<RegisterRequest, UserDto>
{
    private readonly HttpClient _httpClient;
    private readonly IPublisher _publisher;

    public RegisterRequestHandler(HttpClient httpClient, IPublisher publisher)
    {
        _httpClient = httpClient;
        _publisher = publisher;
    }

    public async Task<ErrorOr<UserDto>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request, cancellationToken);
        var result = await response.ToErrorOrResult<UserDto>(cancellationToken);

        if (!result.IsError)
            await _publisher.Publish(new LoginEvent(result.Value), cancellationToken);

        return result;
    }
}
