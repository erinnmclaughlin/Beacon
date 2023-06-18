using Beacon.Common.Auth;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.App;

public static class BeaconApp
{
    public static IServiceCollection AddBeaconCore(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(BeaconApp).Assembly);
        });

        services.AddValidatorsFromAssemblies(new[]
        {
            typeof(BeaconApp).Assembly,
            typeof(LoginRequest).Assembly
        });

        return services;
    }
}
