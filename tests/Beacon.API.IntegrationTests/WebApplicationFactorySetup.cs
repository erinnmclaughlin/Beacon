using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Beacon.API.IntegrationTests;

public static class WebApplicationFactorySetup
{
    public static void ReplaceWithTestDatabase(this IServiceCollection services, string? connectionString)
    {
        services.RemoveAll<DbContextOptions<BeaconDbContext>>();
        services.RemoveAll<BeaconDbContext>();

        services.AddDbContext<BeaconDbContext>(options =>
        {
            options.UseSqlServer(connectionString?.Replace("master", "Beacon"));
        });
    }

    public static void UseMockedCurrentUser(this IServiceCollection services)
    {
        services.RemoveAll<ISessionContext>();
        services.AddSingleton<Mock<ISessionContext>>();
        services.AddScoped(sp => sp.GetRequiredService<Mock<ISessionContext>>().Object);
    }

    public static void UseMockedLabContext(this IServiceCollection services)
    {
        services.RemoveAll<ILabContext>();
        services.AddScoped<ILabContext>(sp =>
        {
            var sessionContext = sp.GetRequiredService<ISessionContext>();
            return new LabContext
            {
                CurrentUser = sessionContext.CurrentUser,
                CurrentLab = sessionContext.CurrentLab!
            };
        });
    }

    public static void UseFakeEmailService(this IServiceCollection services)
    {
        services.RemoveAll<IEmailService>();
        services.AddSingleton<IEmailService, FakeEmailService>();
    }
}