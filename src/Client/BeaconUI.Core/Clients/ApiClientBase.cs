using Beacon.Common;
using BeaconUI.Core.Helpers;
using ErrorOr;
using System.Net.Http.Json;

namespace BeaconUI.Core.Clients;

public abstract class ApiClientBase
{
    private readonly ILabContext _labContext;
    private readonly IHttpClientFactory _httpClientFactory;

    protected ApiClientBase(ILabContext labContext, IHttpClientFactory httpClientFactory)
    {
        _labContext = labContext;
        _httpClientFactory = httpClientFactory;
    }

    protected async Task<ErrorOr<Success>> GetAsync(string requestUri, CancellationToken ct = default)
    {
        using var httpClient = await CreateBeaconClient();
        var response = await httpClient.GetAsync(requestUri, ct);
        return await response.ToErrorOrResult(ct);
    }

    protected async Task<ErrorOr<T>> GetAsync<T>(string requestUri, CancellationToken ct = default)
    {
        using var httpClient = await CreateBeaconClient();
        var response = await httpClient.GetAsync(requestUri, ct);
        return await response.ToErrorOrResult<T>(ct);
    }

    protected async Task<ErrorOr<Success>> PostAsync(string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = await CreateBeaconClient();
        var response = await httpClient.PostAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult(ct);
    }

    protected async Task<ErrorOr<T>> PostAsync<T>(string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = await CreateBeaconClient();
        var response = await httpClient.PostAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult<T>(ct);
    }

    protected async Task<ErrorOr<Success>> PutAsync(string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = await CreateBeaconClient();
        var response = await httpClient.PutAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult(ct);
    }

    protected async Task<ErrorOr<T>> PutAsync<T>(string requestUri, object? requestBody, CancellationToken ct = default)
    {
        using var httpClient = await CreateBeaconClient();
        var response = await httpClient.PutAsJsonAsync(requestUri, requestBody, JsonDefaults.JsonSerializerOptions, ct);
        return await response.ToErrorOrResult<T>(ct);
    }

    private async Task<HttpClient> CreateBeaconClient()
    {
        var labId = await _labContext.GetLaboratoryId();

        var httpClient = _httpClientFactory.CreateBeaconClient();

        if (labId != default)
            httpClient.DefaultRequestHeaders.Add("X-LaboratoryId", labId.ToString());

        return httpClient;
    }
}
