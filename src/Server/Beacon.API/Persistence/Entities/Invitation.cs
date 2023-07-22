using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Invitation : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public required double ExpireAfterDays { get; init; }

    public required string NewMemberEmailAddress { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public Guid? AcceptedById { get; set; }
    public User? AcceptedBy { get; set; }

    public required Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;

    private readonly List<InvitationEmail> _emailInvitations = new();
    public IReadOnlyList<InvitationEmail> EmailInvitations => _emailInvitations.AsReadOnly();

    public InvitationEmail AddEmailInvitation(DateTimeOffset sentOn)
    {
        var invitationEmail = new InvitationEmail
        {
            Id = Guid.NewGuid(),
            LaboratoryInvitationId = Id,
            SentOn = sentOn,
            ExpiresOn = sentOn.AddDays(ExpireAfterDays),
            LaboratoryId = LaboratoryId
        };

        _emailInvitations.Add(invitationEmail);

        return invitationEmail;
    }
}
