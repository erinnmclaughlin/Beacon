using BeaconUI.Core.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services)
    {
        services.AddSingleton(sp => new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5077")
        });

        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddSingleton<AuthenticationStateProvider, BeaconAuthStateProvider>();
        services.AddSingleton<BeaconAuthClient>();

        return services;
    }
}
