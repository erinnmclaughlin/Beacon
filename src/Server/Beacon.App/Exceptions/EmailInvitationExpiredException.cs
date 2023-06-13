namespace Beacon.App.Exceptions;

internal class EmailInvitationExpiredException : BeaconException
{
    public EmailInvitationExpiredException() : base(BeaconExceptionType.BadRequest, "This email invitation has expired.")
    {
    }
}
