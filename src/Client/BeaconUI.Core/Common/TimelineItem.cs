namespace BeaconUI.Core.Common;

public sealed record TimelineItem<T>
{
    public DateTime Timestamp { get; init; }
    public required T Value { get; init; }
}
