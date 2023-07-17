namespace Beacon.API.Persistence.Entities;

public sealed class InvitationEmail : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required DateTimeOffset SentOn { get; init; }
    public required DateTimeOffset ExpiresOn { get; set; }

    public string? OperationId { get; set; }

    public required Guid LaboratoryInvitationId { get; init; }
    public Invitation LaboratoryInvitation { get; init; } = null!;

    public bool IsExpired(DateTimeOffset now)
    {
        return now > ExpiresOn;
    }
}
