using Beacon.Common.Requests;

namespace Beacon.API.IntegrationTests;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> SendAsync<TRequest, TResult>(this HttpClient httpClient, BeaconRequest<TRequest, TResult> request, CancellationToken ct = default)
        where TRequest : BeaconRequest<TRequest, TResult>
    {
        return request.SendAsync(httpClient, ct);
    }
}
