using Beacon.API.Endpoints.Auth;
using Beacon.API.Endpoints.Laboratories;
using Beacon.Common;
using Beacon.Common.Requests;
using Beacon.Common.Requests.Auth;
using Beacon.Common.Validation;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Beacon.API.Endpoints;

public interface IBeaconRequestHandler<TRequest> : IBeaconRequestHandler<TRequest, Success> 
    where TRequest : BeaconRequest<TRequest> { }

public interface IBeaconRequestHandler<TRequest, TResponse> : IBeaconRequestHandlerBase, IRequestHandler<TRequest, ErrorOr<TResponse>>
    where TRequest : BeaconRequest<TRequest, TResponse> { }

public interface IBeaconRequestHandlerBase { }

internal static class EndpointMapper
{
    public static void RegisterBeaconRequestHandlers(this IServiceCollection services)
    {
        var handlerTypes = typeof(LoginHandler).Assembly.GetTypes()
            .Where(t => t.IsClass && t.GetInterfaces()
            .Any(i => i.IsGenericType && i.IsAssignableTo(typeof(IBeaconRequestHandlerBase))));

        foreach (var handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericArguments().Length == 2 && i.IsAssignableTo(typeof(IBeaconRequestHandlerBase)));

            var genericArgumentTypes = interfaceType.GetGenericArguments();
            var (requestType, responseType) = (genericArgumentTypes[0], genericArgumentTypes[1]);

            var genericResponseType = typeof(ErrorOr<>).MakeGenericType(responseType);
            var genericInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, genericResponseType);

            services.AddScoped(genericInterfaceType, handlerType);
        }
    }

    public static void MapBeaconEndpoints(this IEndpointRouteBuilder app)
    {
        var handlerTypes = typeof(LoginHandler).Assembly.GetTypes()
            .Where(t => t.IsClass && t.GetInterfaces()
            .Any(i => i.IsGenericType && i.IsAssignableTo(typeof(IBeaconRequestHandlerBase))));

        foreach (var handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericArguments().Length == 2 && i.IsAssignableTo(typeof(IBeaconRequestHandlerBase)));

            var genericArgumentTypes = interfaceType.GetGenericArguments();
            var (requestType, responseType) = (genericArgumentTypes[0], genericArgumentTypes[1]);

            if (responseType == typeof(Success))
            {
                typeof(EndpointMapper)
                    .GetMethod(nameof(MapPost), new[] { typeof(IEndpointRouteBuilder) })!
                    .MakeGenericMethod(requestType)
                    .Invoke(null, new object[] { app });
            }
            else
            {
                typeof(EndpointMapper)
                    .GetMethod(nameof(MapGet), new[] { typeof(IEndpointRouteBuilder) })!
                    .MakeGenericMethod(requestType, responseType)
                    .Invoke(null, new object[] { app });
            }
        }

    }

    public static void MapPost<TRequest>(IEndpointRouteBuilder app) where TRequest : BeaconRequest<TRequest>, new()
    {
        app.MapPost(typeof(TRequest).Name, async([FromBody] TRequest? request, IMediator m, CancellationToken ct) =>
        {
            var response = await m.Send(request ?? new(), ct);
            return response.ToHttpResult();
        });
    }

    public static void MapGet<TRequest, TResponse>(IEndpointRouteBuilder app) where TRequest : BeaconRequest<TRequest, TResponse>, new()
    {
        app.MapGet(typeof(TRequest).Name, async (string? data, IMediator m, CancellationToken ct) =>
        {
            var request = data is null ? new() : JsonSerializer.Deserialize<TRequest>(data, JsonDefaults.JsonSerializerOptions) ?? new();
            var response = await m.Send(request ?? new(), ct);
            return response.ToHttpResult();
        });
    }

    public static IResult ToHttpResult<T>(this ErrorOr<T> errorOrValue)
    {
        if (!errorOrValue.IsError)
        {
            var value = errorOrValue.Value;
            return value is null || value.GetType() == typeof(Success) ? Results.NoContent() : Results.Ok(value);
        }

        if (errorOrValue.Errors.Where(e => e.Type == ErrorType.Validation).ToList() is { Count: > 0 } validationErrors)
        {
            return Results.UnprocessableEntity(new BeaconValidationProblem
            {
                Errors = validationErrors.GroupBy(v => v.Code)
                    .ToDictionary(x => x.Key, x => x.Select(v => v.Description).ToArray())
            });
        }

        return Results.BadRequest();
    }
}
