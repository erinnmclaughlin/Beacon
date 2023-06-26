using Beacon.API.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Beacon.API.IntegrationTests;

public class DbContextFixture : IAsyncLifetime
{
    public DbConnection Connection { get; }

    public DbContextFixture()
    {
        Connection = new SqliteConnection($"DataSource={Guid.NewGuid()}.db");
    }

    public BeaconDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<BeaconDbContext>();
        optionsBuilder.UseSqlite(Connection);
        return new(optionsBuilder.Options);
    }

    public async Task InitializeAsync()
    {
        await Connection.OpenAsync();

        using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();
        dbContext.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser);
        dbContext.Laboratories.Add(TestData.Lab);
        await dbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await Connection.CloseAsync();

        using (var dbContext = CreateDbContext())
        {
            await dbContext.Database.EnsureDeletedAsync();
        }

        await Connection.DisposeAsync();
    }
}
