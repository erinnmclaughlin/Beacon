namespace DataImporter.Entities;

public sealed class Project
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; set; }
    public required ProjectCode ProjectCode { get; init; }
    public required string ProjectStatus { get; set; }

    public Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = default!;

    public Guid? LeadAnalystId { get; set; }
    public User? LeadAnalyst { get; set; }

    public Guid LaboratoryId { get; set; }
    public Laboratory Laboratory { get; set; } = default!;

    public List<SampleGroup> SampleGroups { get; set; } = new();
}
