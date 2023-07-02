namespace Beacon.Common.Models;

public sealed record ProjectDto
{
    public required Guid Id { get; init; }
    public required string ProjectCode { get; init; }
    public required ProjectStatus ProjectStatus { get; init; }
    public required string CustomerName { get; init; }
    public required LeadAnalystDto? LeadAnalyst { get; init; }

    public sealed record LeadAnalystDto
    {
        public required Guid Id { get; init; }
        public required string DisplayName { get; init; }
    }
}
