using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public class Project : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; set; }
    public required ProjectCode ProjectCode { get; init; }
    public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Active;

    public required Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;

    public Guid? LeadAnalystId { get; set; }
    public User? LeadAnalyst { get; set; }

    public List<ProjectContact> Contacts { get; set; } = new();
    public List<SampleGroup> SampleGroups { get; set; } = new();
}
