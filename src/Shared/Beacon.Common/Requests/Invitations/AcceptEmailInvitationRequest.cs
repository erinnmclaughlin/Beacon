namespace Beacon.Common.Requests.Invitations;

public sealed class AcceptEmailInvitationRequest : BeaconRequest<AcceptEmailInvitationRequest>
{
    public Guid EmailInvitationId { get; set; }
}
