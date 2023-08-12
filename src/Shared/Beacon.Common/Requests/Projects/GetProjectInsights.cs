namespace Beacon.Common.Requests.Projects;

public class GetProjectInsightsRequest : BeaconRequest<GetProjectInsightsRequest, ProjectInsightDto[]>
{
}

public class ProjectInsightDto
{
    public required double Interestingness { get; init; }
    public required string Description { get; init; }
}
