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
    public static IServiceCollection AddBeaconUI(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddAuthorizationCore();
        services.AddScoped<AuthenticationStateProvider, BeaconAuthStateProvider>();
        services.AddScoped(sp => (BeaconAuthStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddScoped<ApiClient>();
        services.AddScoped<AuthService>();

        services.AddBlazoredLocalStorage();
        services.AddScoped<ILabContext, LabContext>();

        services.AddBlazoredModal();

        return services;
    }
}
