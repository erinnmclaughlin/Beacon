using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapPost<TRequest>(this IEndpointRouteBuilder e, string uri)
        where TRequest : IRequest
    {
        return e.MapPost(uri, async (TRequest request, IMediator sender, CancellationToken ct) =>
        {
            await sender.Send(request, ct);
            return Results.NoContent();
        });
    }

    public static RouteHandlerBuilder MapPut<TRequest>(this IEndpointRouteBuilder e, string uri)
        where TRequest : IRequest
    {
        return e.MapPut(uri, async (TRequest request, IMediator sender, CancellationToken ct) =>
        {
            await sender.Send(request, ct);
            return Results.NoContent();
        });
    }

    public static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder e, string uri, IRequest request)
    {
        return e.MapGet(uri, async (IMediator sender, CancellationToken ct) =>
        {
            await sender.Send(request, ct);
            return Results.NoContent();
        });
    }

    public static RouteHandlerBuilder MapGet<TResponse>(this IEndpointRouteBuilder e, string uri, IRequest<TResponse> request)
    {
        return e.MapGet(uri, async (IMediator sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return result == null ? Results.NotFound() : Results.Ok(result);
        });
    }
}
