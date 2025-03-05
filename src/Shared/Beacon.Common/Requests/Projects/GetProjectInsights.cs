﻿namespace Beacon.Common.Requests.Projects;

public class GetProjectInsightsRequest : BeaconRequest<GetProjectInsightsRequest, ProjectInsightDto[]>
{
    public DateOnly ReferenceDate { get; set; } = DateTimeOffset.UtcNow.ToDateOnly().AddMonths(-1);
}

public class ProjectInsightDto
{
    public required InsightType InsightType { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required double Interestingness { get; init; }
}

public enum InsightType
{
    Info,
    SignificantGrowth,
    Growth,
    Decrease,
    SignificantDecrease
}