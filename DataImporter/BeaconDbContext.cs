using DataImporter.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataImporter;

public sealed class BeaconDbContext : DbContext
{
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectContact> ProjectContacts => Set<ProjectContact>();
    public DbSet<ProjectEvent> ProjectEvents => Set<ProjectEvent>();
    public DbSet<SampleGroup> SampleGroups => Set<SampleGroup>();
    public DbSet<User> Users => Set<User>();

    public BeaconDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            builder.OwnsOne(x => x.ProjectCode, b =>
            {
                b.Property(x => x.CustomerCode).HasMaxLength(3);
                b.HasIndex(x => new { x.CustomerCode, x.Suffix });
            });
            builder.Property(x => x.ProjectStatus).HasConversion<string>().HasMaxLength(25);
            builder.HasIndex(x => x.ProjectStatus);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CreatedBy).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectContact>(builder =>
        {
            builder.Property(x => x.EmailAddress).HasMaxLength(255);
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<ProjectEvent>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(x => x.EmailAddress).HasMaxLength(255);
            builder.HasIndex(x => x.EmailAddress).IsUnique();
        });
    }
}
