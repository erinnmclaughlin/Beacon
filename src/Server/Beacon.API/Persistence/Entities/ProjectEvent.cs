namespace Beacon.API.Persistence.Entities;

public sealed class ProjectEvent : LaboratoryScopedEntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public required DateTimeOffset ScheduledStart { get; set; }
    public required DateTimeOffset ScheduledEnd { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;
}
