using Beacon.API.IntegrationTests.Fakes;
using Beacon.API.Persistence;
using Beacon.App.Services;
using Beacon.Common.Models;
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
        services.AddScoped(sp => sp.GetRequiredService<Mock<ISessionContext>>().Object);
    }

    public static void UseMockedLabContext(this IServiceCollection services)
    {
        services.RemoveAll<ILabContext>();
        var mock = new Mock<ILabContext>();
        mock.SetupGet(x => x.CurrentLab).Returns(new CurrentLab
        {
            Id = TestData.Lab.Id,
            Name = TestData.Lab.Name,
            MembershipType = LaboratoryMembershipType.Admin
        });
        services.AddSingleton(_ => mock.Object);
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