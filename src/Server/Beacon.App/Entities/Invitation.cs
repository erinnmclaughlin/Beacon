using Beacon.App.Exceptions;
using Beacon.Common.Models;

namespace Beacon.App.Entities;

public class Invitation : LaboratoryScopedEntityBase
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

    private readonly List<InvitationEmail> _emailInvitations = new();
    public IReadOnlyList<InvitationEmail> EmailInvitations => _emailInvitations.AsReadOnly();

    public bool IsFor(User user)
    {
        return user.EmailAddress == NewMemberEmailAddress;
    }

    public void Accept(User acceptingUser)
    {
        if (!IsFor(acceptingUser))
            throw new UserNotAllowedException("Current user's email address does not match the email address in the invitation.");

        if (!Laboratory.HasMember(acceptingUser))
        {
            AcceptedById = acceptingUser.Id;
            Laboratory.AddMember(acceptingUser.Id, MembershipType);
        }
    }

    public InvitationEmail AddEmailInvitation(DateTimeOffset sentOn)
    {
        var invitationEmail = new InvitationEmail
        {
            Id = Guid.NewGuid(),
            LaboratoryInvitationId = Id,
            SentOn = sentOn,
            ExpiresOn = sentOn.AddDays(ExpireAfterDays),
            LaboratoryId  = LaboratoryId
        };

        _emailInvitations.Add(invitationEmail);

        return invitationEmail;
    }
}
