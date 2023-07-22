using MediatR;
using System.Net.Http.Json;
using System.Text.Json;

namespace Beacon.Common.Requests;

public abstract class BeaconRequestBase<TRequest>
{
    public static bool TryParse(string value, out TRequest result)
    {
        var data = JsonSerializer.Deserialize<TRequest>(value, JsonDefaults.JsonSerializerOptions);
        result = data!;
        return data != null;
    }

    public abstract Task<HttpResponseMessage> SendAsync(HttpClient httpClient, TRequest request, CancellationToken ct = default);
}

public abstract class BeaconRequest<TRequest> : BeaconRequestBase<TRequest>, IRequest where TRequest : BeaconRequest<TRequest>
{
    public override Task<HttpResponseMessage> SendAsync(HttpClient httpClient, TRequest request, CancellationToken ct = default)
    {
        return httpClient.PostAsJsonAsync($"api/{typeof(TRequest).Name}", request, JsonDefaults.JsonSerializerOptions, ct);
    }
}

public abstract class BeaconRequest<TRequest, TResponse> : BeaconRequestBase<TRequest>, IRequest<TResponse> where TRequest : BeaconRequest<TRequest, TResponse>
{
    public override Task<HttpResponseMessage> SendAsync(HttpClient httpClient, TRequest request, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, JsonDefaults.JsonSerializerOptions);
        return httpClient.GetAsync($"api/{typeof(TRequest).Name}?data={json}", ct);
    }
}
