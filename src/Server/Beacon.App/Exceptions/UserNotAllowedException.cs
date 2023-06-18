namespace Beacon.App.Exceptions;

public class UserNotAllowedException : BeaconException
{
    public UserNotAllowedException(string message = "The current user is not authorized to perform this action.")
        : base(BeaconExceptionType.NotAuthorized, message)
    {
    }
}
