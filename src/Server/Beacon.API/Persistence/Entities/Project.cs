using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Project : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; set; }
    public required ProjectCode ProjectCode { get; set; }
    public DateTimeOffset CreatedOn { get; init; } = DateTimeOffset.UtcNow;
    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;

    public Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;

    public Guid? LeadAnalystId { get; set; }
    public User? LeadAnalyst { get; set; }

    public List<ProjectContact> Contacts { get; set; } = [];
    public List<SampleGroup> SampleGroups { get; set; } = [];
    public List<ProjectApplicationTag> TaggedApplications { get; set; } = [];
}
