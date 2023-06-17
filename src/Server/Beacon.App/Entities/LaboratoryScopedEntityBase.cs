namespace Beacon.App.Entities;

public abstract class LaboratoryScopedEntityBase
{
    public Guid LaboratoryId { get; init; }
    public Laboratory Laboratory { get; init; } = null!;
}
