using ErrorOr;
using MediatR;
using System.Net.Http.Json;
using System.Text.Json;

namespace Beacon.Common.Requests;

public abstract class BeaconRequest<TRequest> : BeaconRequest<TRequest, Success>
    where TRequest : BeaconRequest<TRequest>
{
    public override Task<HttpResponseMessage> SendAsync(HttpClient httpClient, CancellationToken ct = default)
    {
        var body = this as TRequest;
        return httpClient.PostAsJsonAsync(Route, body, JsonDefaults.JsonSerializerOptions, ct);
    }
}

public abstract class BeaconRequest<TRequest, TResponse> : IRequest<ErrorOr<TResponse>>
    where TRequest : BeaconRequest<TRequest, TResponse>
{
    public virtual string Route { get; protected set; } = $"api/{typeof(TRequest).Name}";

    public virtual Task<HttpResponseMessage> SendAsync(HttpClient httpClient, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(this as TRequest, JsonSerializerOptions.Default);
        return httpClient.GetAsync(Route + $"?data={json}", ct);
    }
}
