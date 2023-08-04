namespace DataImporter.Entities;

public sealed class ProjectContact
{
    public required Guid Id { get; init; }
    public required string Name { get; set; }
    public required string? EmailAddress { get; set; }
    public required string? PhoneNumber { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid LaboratoryId { get; set; }
    public Laboratory Laboratory { get; set; } = null!;
}
