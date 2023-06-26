namespace Beacon.Common.Models;

public sealed record ProjectContactDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string? EmailAddress { get; init; }
    public required string? PhoneNumber { get; init; }
}
