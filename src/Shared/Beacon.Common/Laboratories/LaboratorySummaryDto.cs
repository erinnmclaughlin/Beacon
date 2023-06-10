namespace Beacon.Common.Laboratories;

public sealed record LaboratorySummaryDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
