namespace Beacon.Common.Requests.Projects;

[RequireMinimumMembership(Models.LaboratoryMembershipType.Member)]
public sealed class GetProjectTypeFrequencyRequest : BeaconRequest<GetProjectTypeFrequencyRequest, GetProjectTypeFrequencyRequest.Series[]>
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.Date);

    public sealed class Series
    {
        public required string ProjectType { get; init; }
        public required Dictionary<DateOnly, int> ProjectCountByDate { get; init; }
    }
}
