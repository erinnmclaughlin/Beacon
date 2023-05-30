using Beacon.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Persistence;

public class BeaconDbContext : DbContext
{
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<LaboratoryMembership> LaboratoryMemberships => Set<LaboratoryMembership>();
    public DbSet<User> Users => Set<User>();

    public BeaconDbContext(DbContextOptions<BeaconDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Laboratory>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<LaboratoryMembership>(builder =>
        {
            builder.HasKey("LaboratoryId", "MemberId");
            builder.Property(x => x.MembershipType).HasConversion<string>().HasMaxLength(20);
            builder.HasOne(x => x.Laboratory).WithMany(x => x.Memberships);
            builder.HasOne(x => x.Member).WithMany(x => x.Memberships);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(x => x.EmailAddress).HasMaxLength(255);
            builder.HasIndex(x => x.EmailAddress).IsUnique();
        });
    }
}
