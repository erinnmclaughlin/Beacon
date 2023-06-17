using Beacon.App.Entities;
using Beacon.App.Services;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Persistence;

public class BeaconDbContext : DbContext, IUnitOfWork, IQueryService
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<InvitationEmail> InvitationEmails => Set<InvitationEmail>();
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Project> Projects => Set<Project>();
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
        modelBuilder.Entity<Customer>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(100);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Invitation>(builder =>
        {
            builder.Property(x => x.NewMemberEmailAddress).HasMaxLength(255);
            builder.Property(x => x.MembershipType).HasConversion<string>().HasMaxLength(20);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CreatedBy).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvitationEmail>(builder =>
        {
            builder.HasOne(x => x.LaboratoryInvitation).WithMany(x => x.EmailInvitations).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Laboratory>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Membership>(builder =>
        {
            builder.HasKey(x => new { x.LaboratoryId, x.MemberId });
            builder.Property(x => x.MembershipType).HasConversion<string>().HasMaxLength(20);
            builder.HasOne(x => x.Laboratory).WithMany(x => x.Memberships).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Member).WithMany(x => x.Memberships);
        });

        modelBuilder.Entity<Project>(builder =>
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectId).IsUnique();
            builder.Property(x => x.ProjectId).HasMaxLength(50);
            builder.HasOne(x => x.Customer).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CreatedBy).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(x => x.EmailAddress).HasMaxLength(255);
            builder.HasIndex(x => x.EmailAddress).IsUnique();
        });
    }
}
