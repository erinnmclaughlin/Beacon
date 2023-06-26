using MediatR;

namespace Beacon.Common.Requests.Invitations;

public sealed class AcceptEmailInvitationRequest : IRequest
{
    public required Guid EmailInvitationId { get; set; }
}
