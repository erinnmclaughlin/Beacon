namespace Beacon.Common.Laboratories;

public sealed record LaboratoryDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}