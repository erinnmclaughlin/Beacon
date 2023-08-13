namespace Beacon.API.Persistence.Entities;

public sealed class ProjectApplication : LaboratoryScopedEntityBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }

    public List<ProjectApplicationTag> TaggedProjects { get; set; } = new();
}
