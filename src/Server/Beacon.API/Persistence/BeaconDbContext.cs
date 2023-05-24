using Beacon.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Persistence;

public class BeaconDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public BeaconDbContext(DbContextOptions<BeaconDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasIndex(x => x.EmailAddress).IsUnique();
            builder.Property(x => x.EmailAddress).HasMaxLength(255);
        });
    }
}
