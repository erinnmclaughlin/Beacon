namespace Beacon.Common.Responses;

public class ValidationProblemResponse
{
    public required Dictionary<string, string[]> Errors { get; init; }
}
