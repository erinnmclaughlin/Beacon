using Beacon.Common;
using Beacon.Common.Auth.Events;
using Beacon.Common.Auth.Requests;
using ErrorOr;
using MediatR;

namespace BeaconUI.Core.Auth.RequestHandlers;

public class LogoutRequestHandler : IApiRequestHandler<LogoutRequest, Success>
{
    private readonly HttpClient _httpClient;
    private readonly IPublisher _publisher;

    public LogoutRequestHandler(HttpClient httpClient, IPublisher publisher)
    {
        _httpClient = httpClient;
        _publisher = publisher;
    }

    public async Task<ErrorOr<Success>> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync("api/auth/logout", cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            await _publisher.Publish(new LogoutEvent(), cancellationToken);
            return Result.Success;
        }

        return Error.Unexpected();
    }
}
