namespace Beacon.Common.Requests.Invitations;

public sealed class AcceptEmailInvitationRequest : BeaconRequest<AcceptEmailInvitationRequest>
{
    public required Guid EmailInvitationId { get; set; }
}
