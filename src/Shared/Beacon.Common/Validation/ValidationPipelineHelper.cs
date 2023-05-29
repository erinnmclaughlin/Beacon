using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Beacon.Common.Validation;

public static class ValidationPipelineHelper
{
    public static void AddValidationPipeline(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        var requestTypes = typeof(ValidationPipelineBehavior<,>).Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IApiRequest<>)));

        foreach (var requestType in requestTypes)
        {
            var interfaceTypes = requestType.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Where(i => i.GetGenericTypeDefinition() == typeof(IApiRequest<>));

            foreach (var interfaceType in interfaceTypes)
            {
                var resultType = interfaceType.GetGenericArguments()[0];
                var wrapperType = typeof(ErrorOr<>).MakeGenericType(resultType);

                var validationPipelineImpl = typeof(ValidationPipelineBehavior<,>).MakeGenericType(requestType, resultType);
                var validationPipelineInterface = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, wrapperType);

                services.AddTransient(validationPipelineInterface, validationPipelineImpl);
            }
        }
    }
}
