using Beacon.Common;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Common.Http;

public static class HttpClientFactoryExtensions
{
    public static async Task<ErrorOr<Success>> GetAsync(this IHttpClientFactory httpClientFactory, string requestUri, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await httpClient.GetAsync(requestUri, ct);
        return await response.ToErrorOrResult(ct);
    }

    public static async Task<ErrorOr<T>> GetAsync<T>(this IHttpClientFactory httpClientFactory, string requestUri, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await httpClient.GetAsync(requestUri, ct);
        return await response.ToErrorOrResult<T>(ct);
    }

    public static async Task<ErrorOr<Success>> PostAsync(this IHttpClientFactory httpClientFactory, string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await httpClient.PostAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult(ct);
    }

    public static async Task<ErrorOr<T>> PostAsync<T>(this IHttpClientFactory httpClientFactory, string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await httpClient.PostAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult<T>(ct);
    }

    public static async Task<ErrorOr<Success>> PutAsync(this IHttpClientFactory httpClientFactory, string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await httpClient.PutAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult(ct);
    }

    public static async Task<ErrorOr<T>> PutAsync<T>(this IHttpClientFactory httpClientFactory, string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await httpClient.PutAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult<T>(ct);
    }

    public static async Task<ErrorOr<Success>> DeleteAsync(this IHttpClientFactory httpClientFactory, string requestUri, CancellationToken ct = default)
    {
        using var httpClient = httpClientFactory.CreateBeaconClient();
        var response = await httpClient.DeleteAsync(requestUri, ct);
        return await response.ToErrorOrResult(ct);
    }

    public static HttpClient CreateBeaconClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient("BeaconApi");
    }
}
