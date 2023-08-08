namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Member)]
public sealed class GetProjectTypeFrequencyRequest : BeaconRequest<GetProjectTypeFrequencyRequest, GetProjectTypeFrequencyRequest.Series[]>
{
    public sealed class Series
    {
        public required string ProjectType { get; init; }
        public required Dictionary<DateOnly, int> ProjectCountByDate { get; init; }
    }
}
