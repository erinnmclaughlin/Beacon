using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Persistence;

public class BeaconDbContext : DbContext, IUnitOfWork, IQueryService
{
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<LaboratoryInvitation> LaboratoryInvitations => Set<LaboratoryInvitation>();
    public DbSet<LaboratoryInvitationEmail> LaboratoryInvitationEmails => Set<LaboratoryInvitationEmail>();
    public DbSet<LaboratoryMembership> LaboratoryMemberships => Set<LaboratoryMembership>();
    public DbSet<User> Users => Set<User>();

    public BeaconDbContext(DbContextOptions<BeaconDbContext> options) : base(options)
    {
    }

    public IRepository<T> GetRepository<T>() where T : class
    {
        return new Repository<T>(this);
    }

    public IQueryable<T> QueryFor<T>() where T : class
    {
        return QueryFor<T>(enableChangeTracking: false);
    }

    public IQueryable<T> QueryFor<T>(bool enableChangeTracking) where T : class
    {
        return enableChangeTracking ? Set<T>() : Set<T>().AsNoTracking();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Laboratory>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<LaboratoryInvitation>(builder =>
        {
            builder.Property(x => x.NewMemberEmailAddress).HasMaxLength(255);
            builder.Property(x => x.MembershipType).HasConversion<string>().HasMaxLength(20);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.CreatedBy).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LaboratoryInvitationEmail>(builder =>
        {
            builder.HasOne(x => x.LaboratoryInvitation).WithMany(x => x.EmailInvitations).OnDelete(DeleteBehavior.Cascade);
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
