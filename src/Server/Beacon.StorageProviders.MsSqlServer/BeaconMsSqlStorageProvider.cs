using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Beacon;

public static class BeaconMsSqlStorageProvider
{
    public static Action<DbContextOptionsBuilder> ConfigureDbContextOptions(string? connectionString) => o =>
    {
        o.UseSqlServer(connectionString, b =>
        {
            b.MigrationsAssembly(typeof(BeaconMsSqlStorageProvider).Assembly);
        });
    };

    public static DbContextOptions<BeaconDbContext> BuildOptions(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BeaconDbContext>();
        ConfigureDbContextOptions(connectionString)(optionsBuilder);
        return optionsBuilder.Options;
    }
}
