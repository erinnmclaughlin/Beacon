namespace Beacon.Common.Validation;

public class BeaconValidationProblem
{
    public required Dictionary<string, string[]> Errors { get; set; }
}
