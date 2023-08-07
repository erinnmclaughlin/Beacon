namespace Beacon.API.Persistence.Entities;

public sealed class ProjectEvent : LaboratoryScopedEntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public required DateTime ScheduledStart { get; set; }
    public required DateTime ScheduledEnd { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public List<LaboratoryInstrument> AssociatedInstruments { get; set; } = new();
}
