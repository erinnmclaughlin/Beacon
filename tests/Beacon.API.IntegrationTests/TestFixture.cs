using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Data.Common;

namespace Beacon.API.IntegrationTests;

[CollectionDefinition(nameof(TestFixture))]
public class TestFixtureCollection : ICollectionFixture<TestFixture> { }

public sealed class TestFixture
{
    public static IServiceScopeFactory BaseScopeFactory { get; private set; } = null!;

    public TestFixture()
    {
        var services = new ServiceCollection();

        // Create open SqliteConnection so EF won't automatically close it.
        services.AddSingleton<DbConnection>(_ =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return connection;
        });

        services.AddBeaconCore();
        services.AddDbContext<BeaconDbContext>((container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection);
        });

        services.AddSingleton(_ = Mock.Of<ISignInManager>());
        services.AddSingleton<Mock<ICurrentUser>>();
        services.AddSingleton(sp => sp.GetRequiredService<Mock<ICurrentUser>>().Object);
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
