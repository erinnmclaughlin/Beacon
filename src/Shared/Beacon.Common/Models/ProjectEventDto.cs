namespace Beacon.Common.Models;

public sealed record ProjectEventDto
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string? Description { get; init; }
    public required DateTimeOffset ScheduledStart { get; init; }
    public required DateTimeOffset ScheduledEnd { get; init; }
    public required LaboratoryInstrumentDto[] AssociatedInstruments { get; init; }

    public TimeSpan GetDuration() => ScheduledEnd - ScheduledStart;

    public bool IsOngoingOn(DateTime timestamp)
    { 
        return ScheduledStart.DateTime <= timestamp && ScheduledEnd.DateTime > timestamp;
    }
}
