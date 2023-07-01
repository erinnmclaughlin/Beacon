namespace Beacon.App.Entities;

public sealed class SampleGroup : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string SampleName { get; set; }
    public required int Quantity { get; set; }
    public required string ContainerType { get; set; }
    public required double Volume { get; set; }
    public required bool IsHazardous { get; set; }
    public bool IsLightSensitive { get; set; }
    public double? TargetStorageTemperature { get; set; }
    public double? TargetStorageHumidity { get; set; }
    public string? Notes { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
