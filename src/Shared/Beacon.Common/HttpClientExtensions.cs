using System.Net.Http.Json;
using Beacon.Common.Requests;

namespace Beacon.Common;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> SendAsync<TRequest>(
        this HttpClient httpClient, 
        BeaconRequest<TRequest> request,
        CancellationToken cancellationToken = default)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new()
        => TRequest.SendAsync(httpClient, request as TRequest ?? new TRequest(), cancellationToken);

    public static Task<HttpResponseMessage> SendAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        BeaconRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken = default)
        where TRequest : BeaconRequest<TRequest, TResponse>, IBeaconRequest<TRequest>, new()
        => TRequest.SendAsync(httpClient, request as TRequest ?? new TRequest(), cancellationToken);

    public static async Task<TResponse?> GetFromJsonAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        BeaconRequest<TRequest, TResponse> request,
        CancellationToken cancellationToken = default)
        where TRequest : BeaconRequest<TRequest, TResponse>, IBeaconRequest<TRequest>, new()
    {
        var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }
}