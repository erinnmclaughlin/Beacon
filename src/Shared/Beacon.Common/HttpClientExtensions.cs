using Beacon.Common.Requests;

namespace Beacon.Common;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> SendAsync<TRequest>(
        this HttpClient httpClient, 
        BeaconRequest<TRequest> request)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new()
        => TRequest.SendAsync(httpClient, request as TRequest ?? new TRequest());

    public static Task<HttpResponseMessage> SendAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        BeaconRequest<TRequest, TResponse> request)
        where TRequest : BeaconRequest<TRequest, TResponse>, IBeaconRequest<TRequest>, new()
        => TRequest.SendAsync(httpClient, request as TRequest ?? new TRequest());
}