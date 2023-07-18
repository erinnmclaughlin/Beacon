namespace Beacon.API.Persistence.Entities;

public abstract class LaboratoryScopedEntityBase
{
    public Guid LaboratoryId { get; set; }
    public Laboratory Laboratory { get; set; } = null!;
}
