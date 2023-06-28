using Beacon.API.Persistence;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Data.Common;

namespace Beacon.API.IntegrationTests;

[CollectionDefinition(nameof(TestFixture))]
public class TestFixtureCollection : ICollectionFixture<TestFixture> { }

public class TestFixture
{
    public static IServiceScopeFactory BaseScopeFactory { get; private set; } = null!;

    static TestFixture()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Configuration.AddConfiguration(new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build());

        var services = builder.Services;

        // Create open SqliteConnection so EF won't automatically close it.
        services.AddSingleton<DbConnection>(container =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return connection;
        });

        services.AddBeaconApi(builder.Configuration, (container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);
        });

        services.RemoveAll<ICurrentUser>();
        services.AddSingleton<Mock<ICurrentUser>>();
        services.AddSingleton(sp => sp.GetRequiredService<Mock<ICurrentUser>>().Object);

        services.RemoveAll<ILabContext>();
        services.AddScoped<ILabContext, TestLabContext>();

        var provider = services.BuildServiceProvider();

        using (var scope = provider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

            db.Database.EnsureCreated();
            db.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser, TestData.NonMemberUser);
            db.Laboratories.Add(TestData.Lab);
            db.SaveChanges();
        }

        BaseScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
    }
}

