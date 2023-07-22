using Beacon.Common.Requests;
using ErrorOr;

namespace BeaconUI.Core.Common.Http;

public interface IApiClient
{
    Task<ErrorOr<Success>> SendAsync<TRequest>(BeaconRequest<TRequest> request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest>;
    Task<ErrorOr<TResult>> SendAsync<TRequest, TResult>(BeaconRequest<TRequest, TResult> request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest, TResult>;
}

internal class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ErrorOr<Success>> SendAsync<TRequest>(BeaconRequest<TRequest> request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest>
    {
        using var httpClient = _httpClientFactory.CreateClient("BeaconApi");
        var response = await request.SendAsync(httpClient, ct);
        return await response.ToErrorOrResult(ct);
    }

    public async Task<ErrorOr<TResult>> SendAsync<TRequest, TResult>(BeaconRequest<TRequest, TResult> request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest, TResult>
    {
        using var httpClient = _httpClientFactory.CreateClient("BeaconApi");
        var response = await request.SendAsync(httpClient, ct);
        return await response.ToErrorOrResult<TResult>(ct);
    }
}
