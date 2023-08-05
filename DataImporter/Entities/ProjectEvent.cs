namespace DataImporter.Entities;

public sealed class ProjectEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public required DateTimeOffset ScheduledStart { get; set; }
    public required DateTimeOffset ScheduledEnd { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public Guid LaboratoryId { get; set; }
    public Laboratory Laboratory { get; set; } = default!;
}
