namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Member)]
public sealed class GetProjectTypeFrequencyRequest : BeaconRequest<GetProjectTypeFrequencyRequest, GetProjectTypeFrequencyRequest.Series[]>
{
    public DateTime StartDate { get; set; } = new(DateTime.Today.Year - 1, DateTime.Today.Month, 1);

    public sealed class Series
    {
        public required string ProjectType { get; init; }
        public required Dictionary<DateOnly, int> ProjectCountByDate { get; init; }
    }
}
