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

    public static RouteHandlerBuilder MapGet<TResponse>(this IEndpointRouteBuilder e, string uri, IRequest<TResponse> request)
    {
        return e.MapGet(uri, request, r => r == null ? Results.NotFound() : Results.Ok(r));
    }

    public static RouteHandlerBuilder MapGet<TResponse>(this IEndpointRouteBuilder e, string uri, IRequest<TResponse> request, Func<TResponse, IResult> handler)
    {
        return e.MapGet(uri, async (IMediator sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return handler.Invoke(result);
        });
    }

    public static RouteHandlerBuilder MapGet<TRequest, TResponse>(this IEndpointRouteBuilder e, string uri)
        where TRequest : IRequest<TResponse>
    {
        return e.MapGet<TRequest, TResponse>(uri, r => r == null ? Results.NotFound() : Results.Ok(r));
    }

    public static RouteHandlerBuilder MapGet<TRequest, TResponse>(this IEndpointRouteBuilder e, string uri, Func<TResponse, IResult> handler)
        where TRequest : IRequest<TResponse>
    {
        return e.MapGet(uri, async (TRequest request, IMediator sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return handler.Invoke(result);
        });
    }
}
