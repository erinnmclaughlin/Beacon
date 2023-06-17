namespace Beacon.Common.Projects;

public sealed record ProjectDto
{
    public required Guid Id { get; init; }
    public required string ProjectCode { get; init; }
    public required string CustomerName { get; init; }
}
