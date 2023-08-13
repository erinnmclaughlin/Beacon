using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Project : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; set; }
    public required ProjectCode ProjectCode { get; set; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;

    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public Guid? LeadAnalystId { get; set; }
    public User? LeadAnalyst { get; set; }

    public List<ProjectContact> Contacts { get; set; } = new();
    public List<SampleGroup> SampleGroups { get; set; } = new();
    public List<ProjectApplicationTag> TaggedApplications { get; set; } = new();
}
