namespace Beacon.API.Persistence.Entities;

public sealed class ProjectApplicationTag : LaboratoryScopedEntityBase
{
    public Guid ApplicationId { get; set; }
    public ProjectApplication Application { get; set; } = default!;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = default!;
}
