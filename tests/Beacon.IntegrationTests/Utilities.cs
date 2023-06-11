using Beacon.API.Persistence;

namespace Beacon.IntegrationTests;

public static class Utilities
{
    public static void EnsureSeeded(this BeaconDbContext db)
    {
        db.Database.EnsureCreated();

        if (!db.Users.Any())
            db.SeedWithTestData();
    }
}
