using Beacon.App.Exceptions;

namespace Beacon.App.Entities;

public class LaboratoryInvitationEmail : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required DateTimeOffset SentOn { get; init; }
    public required DateTimeOffset ExpiresOn { get; init; }

    public string? OperationId { get; set; }

    public required Guid LaboratoryInvitationId { get; init; }
    public LaboratoryInvitation LaboratoryInvitation { get; init; } = null!;

    public void Accept(User acceptingUser, DateTimeOffset acceptedOn)
    {
        if (IsExpired(acceptedOn))
            throw new EmailInvitationExpiredException();

        LaboratoryInvitation.Accept(acceptingUser);
    }

    public bool IsExpired(DateTimeOffset now)
    {
        return now > ExpiresOn;
    }
}
