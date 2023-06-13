namespace Beacon.App.Exceptions;

public class BeaconException : Exception
{
    public BeaconExceptionType Type { get; }

    public BeaconException(BeaconExceptionType type, string message) : base(message)
    {
        Type = type;
    }
}

public enum BeaconExceptionType
{
    NotFound,
    NotAuthorized,
    BadRequest
}
