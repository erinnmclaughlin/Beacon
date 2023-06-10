namespace Beacon.API.Domain.Exceptions;

public sealed class UserNotFoundException : BeaconException
{
    public Guid UserId { get; }

    public UserNotFoundException(Guid userId)
        : base(BeaconExceptionType.NotFound, $"User with id {userId} was not found.")
    {
        UserId = userId;
    }
}
