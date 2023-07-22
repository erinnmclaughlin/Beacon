using Beacon.Common.Requests.Auth;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

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
            var interfaceType = requestType.GetInterfaces().First(i => i.IsAssignableTo(typeof(IBaseRequest)));

            if (interfaceType.IsAssignableTo(typeof(IRequest)))
            {
                typeof(EndpointMapper)
                    .GetMethod(nameof(Map), 1, new[] { typeof(IEndpointRouteBuilder) })!
                    .MakeGenericMethod(requestType)
                    .Invoke(null, new object[] { app });
            }
            else if (interfaceType.GetGenericTypeDefinition() == typeof(IRequest<>))
            {
                var responseType = interfaceType.GetGenericArguments()[0];
                typeof(EndpointMapper)
                    .GetMethod(nameof(Map), 2, new[] { typeof(IEndpointRouteBuilder) })!
                    .MakeGenericMethod(requestType, responseType)
                    .Invoke(null, new object[] { app });
            }
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
        app.MapPost(typeof(TRequest).Name, async ([FromBody] TRequest request, IMediator m, CancellationToken ct) =>
        {
            await m.Send(request, ct);
            return Results.NoContent();
        });
    }

    public static void Map<TRequest, TResponse>(IEndpointRouteBuilder app) where TRequest : IRequest<TResponse>, new()
    {
        app.MapGet(typeof(TRequest).Name, async ([FromQuery] TRequest? request, IMediator m, CancellationToken ct) =>
        {
            var response = await m.Send(request ?? new(), ct);
            return Results.Ok(response);
        });
    }
}
