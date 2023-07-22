using Beacon.Common;
using Beacon.Common.Requests;
using ErrorOr;
using System.Net.Http.Json;
using System.Text.Json;

namespace BeaconUI.Core.Common.Http;

public static class HttpClientExtensions
{
    public static async Task<ErrorOr<Success>> SendAsync<TRequest>(this HttpClient httpClient, BeaconRequest<TRequest> request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest>
    {
        var response = await httpClient.PostAsJsonAsync($"api/{typeof(TRequest).Name}", request as TRequest, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult(ct);
    }

    public static async Task<ErrorOr<TResult>> SendAsync<TRequest, TResult>(this HttpClient httpClient, BeaconRequest<TRequest, TResult> request, CancellationToken ct = default) where TRequest : BeaconRequest<TRequest, TResult>
    {
        var json = JsonSerializer.Serialize(request as TRequest, JsonDefaults.JsonSerializerOptions);
        var response = await httpClient.GetAsync($"api/{typeof(TRequest).Name}?data={json}", ct);
        return await response.ToErrorOrResult<TResult>(ct);
    }
}
