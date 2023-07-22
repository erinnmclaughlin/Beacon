using Beacon.Common.Requests;
using ErrorOr;

namespace BeaconUI.Core.Common.Http;

public interface IApiClient
{
    Task<ErrorOr<Success>> SendAsync<TRequest>(BeaconRequest<TRequest> request, CancellationToken ct = default)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new();

    Task<ErrorOr<TResult>> SendAsync<TRequest, TResult>(BeaconRequest<TRequest, TResult> request, CancellationToken ct = default) 
        where TRequest : BeaconRequest<TRequest, TResult>, IBeaconRequest<TRequest>, new();
}

internal class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorOr<TResult>> SendAsync<TRequest, TResult>(BeaconRequest<TRequest, TResult> request, CancellationToken ct = default)
        where TRequest : BeaconRequest<TRequest, TResult>, IBeaconRequest<TRequest>, new()
    {
        return await SendAsync<TRequest, TResult>(request as TRequest ?? new(), ct);
    }

    public async Task<ErrorOr<Success>> SendAsync<TRequest>(BeaconRequest<TRequest> request, CancellationToken ct)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new()
    {
        return await SendAsync(request as TRequest ?? new(), ct);
    }

    private async Task<ErrorOr<Success>> SendAsync<TRequest>(TRequest request, CancellationToken ct = default)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new()
    {
        using var httpClient = _httpClientFactory.CreateClient("BeaconApi");
        var response = await TRequest.SendAsync(httpClient, request, ct);
        return await response.ToErrorOrResult(ct);
    }

    private async Task<ErrorOr<TResult>> SendAsync<TRequest, TResult>(TRequest request, CancellationToken ct = default)
        where TRequest : BeaconRequest<TRequest, TResult>, IBeaconRequest<TRequest>, new()
    {
        using var httpClient = _httpClientFactory.CreateClient("BeaconApi");
        var response = await TRequest.SendAsync(httpClient, request, ct);
        return await response.ToErrorOrResult<TResult>(ct);
    }

}
