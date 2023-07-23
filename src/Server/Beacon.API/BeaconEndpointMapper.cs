using Beacon.API.Features.Auth;
using Beacon.Common;
using Beacon.Common.Requests;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;

namespace Beacon.API;

public static class BeaconEndpointMapper
{
    public static void MapBeaconEndpoints(this IEndpointRouteBuilder app)
    {
        var handlerTypes = typeof(LoginHandler).Assembly.GetTypes().Where(t => t.IsBeaconRequestHandler());

        foreach (var handlerType in handlerTypes)
        {
            var (requestType, responseType) = handlerType.GetRequestAndResponseTypes();

            if (responseType == typeof(Success))
            {
                typeof(BeaconEndpointMapper)
                    .GetMethod(nameof(MapPost), new[] { typeof(IEndpointRouteBuilder) })!
                    .MakeGenericMethod(requestType)
                    .Invoke(null, new object[] { app });
            }
            else
            {
                typeof(BeaconEndpointMapper)
                    .GetMethod(nameof(MapGet), new[] { typeof(IEndpointRouteBuilder) })!
                    .MakeGenericMethod(requestType, responseType)
                    .Invoke(null, new object[] { app });
            }
        }
    }

    public static void MapPost<TRequest>(IEndpointRouteBuilder app)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new()
    {
        app.MapPost(TRequest.GetRoute(), async ([FromBody] TRequest? request, IMediator m, CancellationToken ct) =>
        {
            var response = await m.Send(request ?? new(), ct);
            return response.ToHttpResult();
        })
        .AddMetaData<TRequest>();
    }

    public static void MapGet<TRequest, TResponse>(IEndpointRouteBuilder app) 
        where TRequest : BeaconRequest<TRequest, TResponse>, IBeaconRequest<TRequest>, new()
    {
        app.MapGet(TRequest.GetRoute(), async (string? data, IMediator m, CancellationToken ct) =>
        {
            var request = JsonSerializer.Deserialize<TRequest>(data ?? "", JsonDefaults.JsonSerializerOptions);
            var response = await m.Send(request ?? new(), ct);
            return response.ToHttpResult();
        })
        .AddMetaData<TRequest>();
    }

    public static void AddMetaData<TRequest>(this RouteHandlerBuilder builder)
    {
        if (typeof(TRequest).Namespace is { } requestNamespace)
        {
            builder.WithTags(requestNamespace[(requestNamespace.LastIndexOf(".") + 1)..]);
        }
    }
}