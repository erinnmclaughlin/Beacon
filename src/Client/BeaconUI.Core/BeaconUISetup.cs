using Beacon.Common.Services;
using BeaconUI.Core.Common.Auth;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services, string baseUri)
    {
        services
            .AddHttpClient("BeaconApi", o => o.BaseAddress = new Uri(baseUri));

        services
            .AddOptions()
            .AddAuthorizationCore()
            .AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>()
            .AddScoped(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
            .AddScoped<ISessionContext>(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
            .AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BeaconApi"))
            .AddScoped<AuthService>()
            .AddBlazoredModal();

        return services;
    }
}
