namespace Beacon.API.Domain.Exceptions;

public class UserNotAllowedException : BeaconException
{
    public UserNotAllowedException(string message) 
        : base(BeaconExceptionType.NotAuthorized, message)
    {
    }
}
