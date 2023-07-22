namespace Beacon.Common.Services;

public class LabContext : ILabContext
{
    public required CurrentUser CurrentUser { get; init; }
    public required CurrentLab CurrentLab { get; init; }
}