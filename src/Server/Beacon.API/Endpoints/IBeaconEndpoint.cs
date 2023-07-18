using Beacon.Common.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace Beacon.API.Endpoints;

public interface IBeaconEndpoint
{
    static abstract void Map(IEndpointRouteBuilder app);
}

internal static class EndpointMapper
{
    public static void MapBeaconEndpoints(this IEndpointRouteBuilder app)
    {
        var requestTypes = typeof(LoginRequest).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsAssignableTo(typeof(IBaseRequest))));

        foreach (var requestType in requestTypes)
        {

        }

        //var endpoints = Assembly.GetExecutingAssembly().GetTypes()
        //    .Where(t => t.GetInterfaces().Any(i => i == typeof(IBeaconEndpoint)));

        //foreach (var endpoint in endpoints)
        //{
        //    var mapMethod = endpoint.GetMethod(nameof(IBeaconEndpoint.Map), types: new[] { typeof(IEndpointRouteBuilder) });
        //    mapMethod!.Invoke(null, new object[] { app });
        //}
    
    }

    public static void Map<TRequest>(IEndpointRouteBuilder app) where TRequest : IRequest
    {
        app.MapPost($"api/{typeof(TRequest).Name}", async (TRequest request, IMediator m, CancellationToken ct) =>
        {
            await m.Send(request, ct);
            return Results.NoContent();
        });
    }

    public static void Map<TRequest, TResponse>(IEndpointRouteBuilder app) where TRequest : IRequest<TResponse>
    {
        app.MapPost($"api/{typeof(TRequest).Name}", async (TRequest request, IMediator m, CancellationToken ct) =>
        {
            var response = await m.Send(request, ct);
            return Results.Ok(response);
        });
    }
}