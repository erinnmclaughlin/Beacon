namespace Beacon.API.Domain.Entities;

public class User
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; set; }
    public required string EmailAddress { get; set; }
    public required string HashedPassword { get; set; }
    public required byte[] HashedPasswordSalt { get; set; }

    private readonly List<LaboratoryMembership> _memberships = new();
    public IReadOnlyList<LaboratoryMembership> Memberships => _memberships;
}
