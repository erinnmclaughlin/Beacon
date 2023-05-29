using Beacon.Common.Validation;
using BeaconUI.Core.Auth.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BeaconUI.Core.Helpers;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddValidationPipeline();

        return services;
    }
}
