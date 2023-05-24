using Beacon.API;
using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.API.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Data.Common;

namespace Beacon.IntegrationTests;

public class BeaconTestApplicationFactory : WebApplicationFactory<BeaconAPI>
{
    public BeaconTestApplicationFactory()
    {
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<BeaconDbContext>().Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<BeaconDbContext>>();
            services.RemoveAll<DbConnection>();

            services.AddDbContext<BeaconDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryBeaconDb");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
            dbContext.Database.EnsureCreated();
        });

        builder.UseEnvironment("Development");
    }

    public async Task SeedDbWithUserData(string email, string displayName, string password)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            EmailAddress = email,
            DisplayName = displayName,
            HashedPassword = passwordHasher.Hash(password, out var salt),
            HashedPasswordSalt = salt
        });

        await db.SaveChangesAsync();
    }
}
