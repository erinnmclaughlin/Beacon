namespace Beacon.API.Persistence.Entities;

public abstract class LaboratoryScopedEntityBase
{
    public Guid LaboratoryId { get; init; }
    public Laboratory Laboratory { get; init; } = null!;
}
