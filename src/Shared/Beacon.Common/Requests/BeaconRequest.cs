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
        return httpClient.PostAsJsonAsync(GetRoute(), this as TRequest, JsonDefaults.JsonSerializerOptions, ct);
    }

    public override string GetRoute()
    {
        return $"api/commands/{GetName()}";
    }
}

public abstract class BeaconRequest<TRequest, TResponse> : IRequest<ErrorOr<TResponse>>
    where TRequest : BeaconRequest<TRequest, TResponse>
{
    public virtual Task<HttpResponseMessage> SendAsync(HttpClient httpClient, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(this as TRequest, JsonSerializerOptions.Default);
        return httpClient.GetAsync(GetRoute() + $"?data={json}", ct);
    }

    public virtual string GetRoute()
    {
        return $"api/queries/{GetName()}";
    }

    public virtual string GetName()
    {
        var name = typeof(TRequest).Name.Replace("Request", "");
        return name[0..1].ToLower() + name[1..];
    }
}
