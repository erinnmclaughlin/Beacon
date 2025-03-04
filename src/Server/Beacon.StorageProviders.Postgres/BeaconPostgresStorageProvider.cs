using Beacon.API;
using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Beacon;

public static class BeaconPostgresStorageProvider
{
    public static Action<DbContextOptionsBuilder> BuildDbContextOptionsBuilder(string? connectionString) => o =>
    {
        o.UseSnakeCaseNamingConvention();
        o.UseNpgsql(connectionString, b =>
        {
            b.MigrationsAssembly(typeof(BeaconPostgresStorageProvider).Assembly);
        });
    };

    public static DbContextOptions<BeaconDbContext> BuildOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BeaconDbContext>();
        BuildDbContextOptionsBuilder(connectionString)(optionsBuilder);
        return optionsBuilder.Options;
    }
}
