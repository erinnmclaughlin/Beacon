namespace Beacon.API.Exceptions;

public class UserNotAllowedException : ApplicationException
{
    public const string DefaultMessage = "The current user is not authorized to perform this action.";

    public UserNotAllowedException(string message = DefaultMessage) : base(message)
    {
    }
}
