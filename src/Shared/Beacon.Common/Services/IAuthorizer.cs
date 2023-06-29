using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Beacon.Common.Services;

public interface IAuthorizer<TRequest>
{
    Task<bool> IsAuthorizedAsync(TRequest request, CancellationToken ct = default);
}

public static class AuthorizerLocator
{
    public static void RegisterAuthorizers(this IServiceCollection services)
    {
        var authorizers = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAuthorizer<>)));

        foreach (var authorizer in authorizers)
        {
            var abstraction = authorizer.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAuthorizer<>));
            services.AddScoped(abstraction, authorizer);
        }
    }
}