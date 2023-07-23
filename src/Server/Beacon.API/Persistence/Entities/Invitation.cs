using Beacon.Common.Models;

namespace Beacon.API.Persistence.Entities;

public sealed class Invitation : LaboratoryScopedEntityBase
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset CreatedOn { get; init; } = DateTimeOffset.UtcNow;
    public double ExpireAfterDays { get; init; } = 10;

    public required string NewMemberEmailAddress { get; init; }
    public required LaboratoryMembershipType MembershipType { get; init; }

    public Guid? AcceptedById { get; set; }
    public User? AcceptedBy { get; set; }

    public required Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;

    public List<InvitationEmail> EmailInvitations { get; set; } = new();
}
