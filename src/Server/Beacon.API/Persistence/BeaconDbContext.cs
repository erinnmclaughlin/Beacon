using Beacon.API.Persistence.Entities;
using Beacon.Common.Services;
using Beacon.Common.Validation.Rules;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Persistence;

public class BeaconDbContext(DbContextOptions options, ISessionContext sessionContext) : DbContext(options)
{
    private readonly ISessionContext _sessionContext = sessionContext;

    public DbSet<Invitation> Invitations => Set<Invitation>();
    public DbSet<InvitationEmail> InvitationEmails => Set<InvitationEmail>();
    public DbSet<Laboratory> Laboratories => Set<Laboratory>();
    public DbSet<LaboratoryInstrument> LaboratoryInstruments => Set<LaboratoryInstrument>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectApplication> ProjectApplications => Set<ProjectApplication>();
    public DbSet<ProjectApplicationTag> ProjectApplicationTags => Set<ProjectApplicationTag>();
    public DbSet<ProjectContact> ProjectContacts => Set<ProjectContact>();
    public DbSet<ProjectEvent> ProjectEvents => Set<ProjectEvent>();
    public DbSet<SampleGroup> SampleGroups => Set<SampleGroup>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invitation>(builder =>
        {
            builder.Property(x => x.NewMemberEmailAddress).HasMaxLength(255);
            builder.Property(x => x.MembershipType).HasConversion<string>().HasMaxLength(20);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CreatedBy).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
        });

        modelBuilder.Entity<InvitationEmail>(builder =>
        {
            builder.HasOne(x => x.LaboratoryInvitation).WithMany(x => x.EmailInvitations).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
        });

        modelBuilder.Entity<Laboratory>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<LaboratoryInstrument>(builder =>
        {
            builder.Property(x => x.SerialNumber).HasMaxLength(100);
            builder.Property(x => x.InstrumentType).HasMaxLength(100);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);

            builder
                .HasMany(x => x.AssociatedEvents)
                .WithMany(x => x.AssociatedInstruments)
                .UsingEntity<LaboratoryInstrumentUsage>(
                    configureLeft: l => l.HasOne(e => e.Instrument).WithMany().HasForeignKey(e => e.InstrumentId),
                    configureRight: r => r.HasOne(e => e.ProjectEvent).WithMany().HasForeignKey(e => e.ProjectEventId)
                );
        });

        modelBuilder.Entity<LaboratoryInstrumentUsage>(builder =>
        {
            builder.HasKey(x => new { x.InstrumentId, x.ProjectEventId });
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Membership>(builder =>
        {
            builder.HasKey(x => new { x.LaboratoryId, x.MemberId });
            builder.Property(x => x.MembershipType).HasConversion<string>().HasMaxLength(20);
            builder.HasOne(x => x.Laboratory).WithMany(x => x.Memberships).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Member).WithMany(x => x.Memberships);
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
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
            builder.HasOne(x => x.Laboratory).WithMany(x => x.Projects).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.CreatedBy).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
        });

        modelBuilder.Entity<ProjectApplication>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(50);
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectApplicationTag>(builder =>
        {
            builder.HasKey(x => new { x.ApplicationId, x.ProjectId });
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectContact>(builder =>
        {
            builder.Property(x => x.Name).HasMaxLength(ContactRules.MaximumNameLength).IsRequired();
            builder.Property(x => x.EmailAddress).HasMaxLength(255);
            builder.Property(x => x.PhoneNumber).HasMaxLength(50);
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectEvent>(builder =>
        {
            builder.Property(x => x.Title).HasMaxLength(50);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
        });

        modelBuilder.Entity<SampleGroup>(builder =>
        {
            builder.HasQueryFilter(x => x.LaboratoryId == _sessionContext.CurrentLab!.Id);
            builder.HasOne(x => x.Laboratory).WithMany().OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(x => x.EmailAddress).HasMaxLength(255);
            builder.HasIndex(x => x.EmailAddress).IsUnique();
        });
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        UpdateLaboratoryScopedEntities();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken ct = default)
    {
        UpdateLaboratoryScopedEntities();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, ct);
    }

    private void UpdateLaboratoryScopedEntities()
    {
        var entries = ChangeTracker.Entries<LaboratoryScopedEntityBase>();

        foreach (var e in entries.Where(e => e.State == EntityState.Added))
        {
            if (e.Entity.LaboratoryId == default)
                e.Entity.LaboratoryId = _sessionContext.CurrentLab?.Id ?? default;
        }
    }
}
