namespace DataImporter.Entities;

public sealed class Laboratory
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }

    public List<Membership> Memberships { get; set; } = new();
}