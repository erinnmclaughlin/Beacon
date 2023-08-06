namespace Beacon.Common.Models;

public sealed record LaboratoryEventDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required DateTime ScheduledStart { get; init; }
    public required DateTime ScheduledEnd { get; init; }
    public required ProjectCode ProjectCode { get; init; }
    public required LaboratoryInstrumentDto[] AssociatedInstruments { get; init; }

    public bool IsCompletedOnOrBefore(DateTime timestamp)
    {
        return ScheduledEnd <= timestamp;
    }

    public bool IsOngoingDuring(DateTime timestamp)
    {
        return ScheduledStart <= timestamp && ScheduledEnd > timestamp;
    }
}
