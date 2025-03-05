namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Member)]
public sealed class GetProjectTypeFrequencyRequest : BeaconRequest<GetProjectTypeFrequencyRequest, GetProjectTypeFrequencyRequest.Series[]>
{
    public DateOnly StartDate { get; set; } = new(DateTime.UtcNow.Year - 1, DateTime.UtcNow.Month, 1);

    public sealed class Series
    {
        public required string ProjectType { get; init; }
        public required Dictionary<DateOnly, int> ProjectCountByDate { get; init; }
    }
}
