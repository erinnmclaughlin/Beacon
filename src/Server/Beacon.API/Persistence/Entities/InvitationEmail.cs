namespace Beacon.API.Persistence.Entities;

public sealed class InvitationEmail : LaboratoryScopedEntityBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset SentOn { get; init; } = DateTimeOffset.UtcNow;
    public required DateTimeOffset ExpiresOn { get; set; }

    public string? OperationId { get; set; }

    public Guid LaboratoryInvitationId { get; init; }
    public Invitation LaboratoryInvitation { get; init; } = null!;

    public bool IsExpired(DateTimeOffset asOfDate)
    {
        return asOfDate > ExpiresOn;
    }
}
