using Beacon.App.Exceptions;
using Beacon.Common.Laboratories.Enums;

namespace Beacon.App.Entities;

public class LaboratoryInvitation : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required DateTimeOffset CreatedOn { get; init; }
    public required double ExpireAfterDays { get; init; }

    public required string NewMemberEmailAddress { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public Guid? AcceptedById { get; private set; }
    public User? AcceptedBy { get; private set; }

    public required Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;

    private readonly List<LaboratoryInvitationEmail> _emailInvitations = new();
    public IReadOnlyList<LaboratoryInvitationEmail> EmailInvitations => _emailInvitations.AsReadOnly();

    public void Accept(User acceptingUser)
    {
        if (acceptingUser.EmailAddress != NewMemberEmailAddress)
            throw new UserNotAllowedException("Current user's email address does not match the email address in the invitation.");

        AcceptedById = acceptingUser.Id;
    }

    public LaboratoryInvitationEmail AddEmailInvitation(DateTimeOffset sentOn)
    {
        var invitationEmail = new LaboratoryInvitationEmail
        {
            Id = Guid.NewGuid(),
            LaboratoryInvitationId = Id,
            SentOn = sentOn,
            ExpiresOn = sentOn.AddDays(ExpireAfterDays)
        };

        _emailInvitations.Add(invitationEmail);

        return invitationEmail;
    }
}
