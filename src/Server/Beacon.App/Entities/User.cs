namespace Beacon.App.Entities;

public class User
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; set; }
    public required string EmailAddress { get; set; }
    public required string HashedPassword { get; set; }
    public required byte[] HashedPasswordSalt { get; set; }

    private readonly List<Membership> _memberships = new();
    public IReadOnlyList<Membership> Memberships => _memberships;
}
