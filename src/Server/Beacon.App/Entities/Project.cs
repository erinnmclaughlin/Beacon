namespace Beacon.App.Entities;

public class Project : LaboratoryScopedEntityBase
{
    public Guid Id { get; init; }
    public string ProjectId { get; init; } = string.Empty;

    public Guid CustomerId { get; init; }
    public Customer Customer { get; init; } = null!;

    public Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;
}
