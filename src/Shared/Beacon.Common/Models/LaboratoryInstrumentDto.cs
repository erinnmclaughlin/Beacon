namespace Beacon.Common.Models;

public sealed record LaboratoryInstrumentDto
{
    public required Guid Id { get; init; }
    public required string SerialNumber { get; init; }
    public required string InstrumentType { get; init; }
}
