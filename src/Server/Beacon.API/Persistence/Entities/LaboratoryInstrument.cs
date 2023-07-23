namespace Beacon.API.Persistence.Entities;

public sealed class LaboratoryInstrument : LaboratoryScopedEntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string SerialNumber { get; set; }
    public required string InstrumentType { get; set; }
}
