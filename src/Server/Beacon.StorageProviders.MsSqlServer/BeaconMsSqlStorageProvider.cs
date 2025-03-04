using Beacon.API;
using Beacon.API.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Beacon;

public static class BeaconMsSqlStorageProvider
{
    public static WebApplicationBuilder AddBeaconApi(this WebApplicationBuilder builder, string connectionStringName = "BeaconDb")
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{connectionStringName}' was not found in configuration.");
        }

        builder.Services.AddBeaconApi(builder.Configuration, BuildDbContextOptionsBuilder(connectionString));

        return builder;
    }

    public static Action<DbContextOptionsBuilder> BuildDbContextOptionsBuilder(string connectionString) => o =>
    {
        o.UseSqlServer(connectionString, b =>
        {
            b.MigrationsAssembly(typeof(BeaconMsSqlStorageProvider).Assembly);
        });
    };

    public static DbContextOptions<BeaconDbContext> BuildOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BeaconDbContext>();
        BuildDbContextOptionsBuilder(connectionString)(optionsBuilder);
        return optionsBuilder.Options;
    }
}
