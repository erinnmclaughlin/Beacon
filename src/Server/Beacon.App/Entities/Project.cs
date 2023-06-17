using Beacon.App.ValueObjects;

namespace Beacon.App.Entities;

public class Project : LaboratoryScopedEntityBase
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; set; }
    public required ProjectCode ProjectCode { get; init; }

    public Guid CreatedById { get; init; }
    public User CreatedBy { get; init; } = null!;
}
