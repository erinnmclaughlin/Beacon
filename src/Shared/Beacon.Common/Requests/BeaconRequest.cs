using ErrorOr;
using MediatR;
using System.Net.Http.Json;
using System.Text.Json;

namespace Beacon.Common.Requests;

public interface IBeaconRequest
{
    static abstract string GetRoute();
}

public interface IBeaconRequest<TRequest> : IBeaconRequest where TRequest : IBeaconRequest<TRequest>
{
    static abstract Task<HttpResponseMessage> SendAsync(HttpClient httpClient, TRequest request, CancellationToken ct = default);

    public static virtual string GetName()
    {
        var name = typeof(TRequest).Name.Replace("Request", "");
        return name[0..1].ToLower() + name[1..];
    }
}

public abstract class BeaconRequest<TRequest> : 
    IBeaconRequest<TRequest>, 
    IRequest<ErrorOr<Success>> where TRequest : 
    IBeaconRequest<TRequest>
{
    public static Task<HttpResponseMessage> SendAsync(HttpClient httpClient, TRequest request, CancellationToken ct = default)
    {
        return httpClient.PostAsJsonAsync(GetRoute(), request, JsonDefaults.JsonSerializerOptions, ct);
    }

    public static string GetRoute()
    {
        return $"api/commands/{TRequest.GetName()}";
    }
}

public abstract class BeaconRequest<TRequest, TResponse> : 
    IBeaconRequest<TRequest>, 
    IRequest<ErrorOr<TResponse>> where TRequest : 
    IBeaconRequest<TRequest>
{
    public static Task<HttpResponseMessage> SendAsync(HttpClient httpClient, TRequest request, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, JsonSerializerOptions.Default);
        return httpClient.GetAsync(GetRoute() + $"?data={json}", ct);
    }

    public static string GetRoute()
    {
        return $"api/queries/{TRequest.GetName()}";
    }
}
