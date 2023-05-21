using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services)
    {
        services.AddScoped(sp => new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7198")
        });

        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>();

        return services;
    }
}
