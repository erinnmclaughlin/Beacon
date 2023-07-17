﻿using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
        services.RemoveAll<ISessionContext>();
        services.AddSingleton<Mock<ISessionContext>>();
        services.AddSingleton(sp => sp.GetRequiredService<Mock<ISessionContext>>().Object);
    }

    public static void UseMockedLabContext(this IServiceCollection services)
    {
        services.RemoveAll<ILabContext>();
        services.AddSingleton<ILabContext>(sp =>
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

    public static void SuppressLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
    }
}