namespace Beacon.API.Persistence.Entities;

public class User
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; set; }
    public required string EmailAddress { get; set; }
    public required string HashedPassword { get; set; }
    public required byte[] HashedPasswordSalt { get; set; }

    public List<Membership> Memberships { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
}
