namespace Beacon.API.Persistence.Entities;

public class LaboratoryInstrumentUsage : LaboratoryScopedEntityBase
{
    public Guid InstrumentId { get; init; }
    public LaboratoryInstrument Instrument { get; init; } = null!;

    public Guid ProjectEventId { get; init; }
    public ProjectEvent ProjectEvent { get; init; } = null!;
}
