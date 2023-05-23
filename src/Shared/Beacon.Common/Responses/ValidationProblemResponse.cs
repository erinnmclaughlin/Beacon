namespace Beacon.Common.Responses;

public class ValidationProblemResponse
{
    public string? Detail { get; set; }
    public required Dictionary<string, string[]> Errors { get; init; }
}
