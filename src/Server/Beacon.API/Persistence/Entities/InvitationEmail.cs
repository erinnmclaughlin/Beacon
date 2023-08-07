namespace Beacon.API.Persistence.Entities;

public sealed class InvitationEmail : LaboratoryScopedEntityBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime SentOn { get; init; } = DateTime.UtcNow;
    public required DateTime ExpiresOn { get; set; }

    public string? OperationId { get; set; }

    public Guid LaboratoryInvitationId { get; init; }
    public Invitation LaboratoryInvitation { get; init; } = null!;

    public bool IsExpired(DateTime now)
    {
        return now > ExpiresOn;
    }
}
