using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace Beacon.API.IntegrationTests;

public class BeaconTestApplicationFactory : WebApplicationFactory<BeaconAPI>
{
    public BeaconTestApplicationFactory()
    {
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<BeaconDbContext>().Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<BeaconDbContext>>();
            services.RemoveAll<DbConnection>();

            // Create open SqliteConnection so EF won't automatically close it.
            services.AddSingleton<DbConnection>(container =>
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
        });

        builder.UseEnvironment("Development");
    }

    public async Task ResetTestDb()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task<User> AddUser(string email, string displayName, string password)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var hashedPassword = passwordHasher.Hash(password, out var salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = email,
            DisplayName = displayName,
            HashedPassword = hashedPassword,
            HashedPasswordSalt = salt
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }
}
