namespace Beacon.Common.Models;

public sealed record SampleGroupDto
{
    public required Guid Id { get; init; }
    public required string SampleName { get; init; }
    public required int? Quantity { get; init; }
    public required string? ContainerType { get; init; }
    public required double? Volume { get; init; }
    public required bool? IsHazardous { get; init; }
    public required bool? IsLightSensitive { get; init; }
    public required double? TargetStorageTemperature { get; init; }
    public required double? TargetStorageHumidity { get; init; }
    public required string? Notes { get; init; }
}
