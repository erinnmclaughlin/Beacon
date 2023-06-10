namespace Beacon.API.Domain.Exceptions;

public class EmailInvitationNotFoundException : BeaconException
{
    public Guid EmailId { get; }
    public Guid? InviteId { get; }

    public EmailInvitationNotFoundException(Guid emailId, Guid? inviteId = null)
        : base(BeaconExceptionType.NotFound, "Email invitation was not found.")
    {
        EmailId = emailId;
        InviteId = inviteId;
    }
}
