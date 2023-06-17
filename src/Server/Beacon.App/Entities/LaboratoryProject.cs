namespace Beacon.App.Entities;

public class LaboratoryProject : LaboratoryScopedEntityBase
{
    public Guid Id { get; init; }
    public string ProjectId { get; init; } = string.Empty;

    public Guid CustomerId { get; init; }
    public LaboratoryCustomer Customer { get; init; } = null!;

    public Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;
}
