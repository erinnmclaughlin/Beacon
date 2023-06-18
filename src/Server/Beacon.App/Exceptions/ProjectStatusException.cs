namespace Beacon.App.Exceptions;

public sealed class ProjectStatusException : BeaconException
{
    public ProjectStatusException(string message) : base(BeaconExceptionType.BadRequest, message)
    {
    }
}
