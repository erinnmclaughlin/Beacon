using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Beacon.API.IntegrationTests;

public static class WebApplicationFactorySetup
{
    public static void ReplaceWithTestDatabase(this IServiceCollection services, WebHostBuilderContext? context, string? connectionString)
    {
        services.RemoveAll<DbContextOptions<BeaconDbContext>>();
        services.RemoveAll<BeaconDbContext>();

        var storageProvider = context?.Configuration["StorageProvider"]!;

        services.AddDbContext<BeaconDbContext>(BeaconWebHost.ConfigureDbContextOptions(storageProvider, connectionString));
    }

    public static void UseFakeEmailService(this IServiceCollection services)
    {
        services.RemoveAll<IEmailService>();
        services.AddSingleton<IEmailService, FakeEmailService>();
    }
}