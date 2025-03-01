namespace Beacon.API.Persistence.Entities;

public sealed class ProjectApplicationTag : LaboratoryScopedEntityBase
{
    public ProjectApplicationTag()
    {
    }

    public ProjectApplicationTag(Guid applicationId, Guid laboratoryId)
    {
        ApplicationId = applicationId;
        LaboratoryId = laboratoryId;
    }

    public Guid ApplicationId { get; init; }
    public ProjectApplication Application { get; init; } = default!;

    public Guid ProjectId { get; init; }
    public Project Project { get; init; } = default!;
}
