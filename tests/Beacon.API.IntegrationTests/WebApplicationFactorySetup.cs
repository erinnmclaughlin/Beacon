using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence;
using Beacon.App.Services;
using Beacon.Common.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace Beacon.API.IntegrationTests;

public static class WebApplicationFactorySetup
{
    public static void ReplaceWithTestDatabase(this IServiceCollection services)
    {
        services.RemoveAll<DbContextOptions<BeaconDbContext>>();
        services.RemoveAll<BeaconDbContext>();

        // Create open SqliteConnection so EF won't automatically close it.
        services.AddSingleton<DbConnection>(_ =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return connection;
        });

        services.AddDbContext<BeaconDbContext>((container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);
        });
    }

    public static void UseMockedCurrentUser(this IServiceCollection services)
    {
        services.RemoveAll<ICurrentUser>();
        services.AddSingleton<Mock<ICurrentUser>>();
        services.AddScoped(sp => sp.GetRequiredService<Mock<ICurrentUser>>().Object);
    }

    public static void UseFakeLabContext(this IServiceCollection services)
    {
        services.RemoveAll<ILabContext>();
        services.AddScoped<ILabContext, FakeLabContext>();
    }

    public static void UseFakeEmailService(this IServiceCollection services)
    {
        services.RemoveAll<IEmailService>();
        services.AddSingleton<IEmailService, FakeEmailService>();
    }
}