namespace Beacon.API.Persistence.Entities;

public class LaboratoryInstrumentUsage : LaboratoryScopedEntityBase
{
    public Guid InstrumentId { get; set; }
    public LaboratoryInstrument Instrument { get; set; } = default!;

    public Guid ProjectEventId { get; set; }
    public ProjectEvent ProjectEvent { get; set; } = default!;
}
