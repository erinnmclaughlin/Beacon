using BeaconUI.Core.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services)
    {
        services.AddScoped<CookieHandler>();

        services.AddHttpClient("BeaconAPI", options =>
        {
            options.BaseAddress = new Uri("https://localhost:7198/");
        })
            .AddHttpMessageHandler<CookieHandler>();

        services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BeaconAPI"));

        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>();
        services.AddScoped<BeaconAuthClient>();

        return services;
    }
}
