using Beacon.API.Behaviors;
using Beacon.API.Features.Auth;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API;

internal static class BeaconRequestHandlerRegistration
{
    public static void RegisterBeaconRequestHandlers(this IServiceCollection services)
    {
        var handlerTypes = typeof(LoginHandler).Assembly.GetTypes().Where(t => t.IsBeaconRequestHandler());
        
        foreach (var handlerType in handlerTypes)
        {
            var (requestType, responseType) = handlerType.GetRequestAndResponseTypes();

            var genericResponseType = typeof(ErrorOr<>).MakeGenericType(responseType);
            var genericInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(requestType, genericResponseType);

            services.AddScoped(genericInterfaceType, handlerType);

            var pipelineBehaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, genericResponseType);
            services.AddScoped(pipelineBehaviorType, typeof(AuthorizationPipelineBehavior<,>).MakeGenericType(requestType, responseType));
            services.AddScoped(pipelineBehaviorType, typeof(ValidationPipelineBehavior<,>).MakeGenericType(requestType, responseType));
        }
    }
}
