namespace Beacon.App.Entities;

public sealed class Customer : LaboratoryScopedEntityBase
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
