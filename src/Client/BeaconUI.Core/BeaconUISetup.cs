using Beacon.Common;
using BeaconUI.Core.Clients;
using BeaconUI.Core.Services;
using Blazored.LocalStorage;
using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace BeaconUI.Core;

public static class BeaconUISetup
{
    public static IServiceCollection AddBeaconUI(this IServiceCollection services, string baseUri)
    {
        services
            .AddTransient<BeaconHttpHeaderHandler>()
            .AddHttpClient("BeaconApi", o => o.BaseAddress = new Uri(baseUri))
            .AddHttpMessageHandler<BeaconHttpHeaderHandler>();

        services
            .AddOptions()
            .AddAuthorizationCore()
            .AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>()
            .AddScoped(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>())
            .AddScoped<ICurrentUser, BeaconAuthStateProvider>()
            .AddScoped<ApiClient>()
            .AddScoped<AuthService>()
            .AddScoped<ILabContext, LabContext>()
            .AddBlazoredLocalStorage()
            .AddBlazoredModal();

        return services;
    }
}
