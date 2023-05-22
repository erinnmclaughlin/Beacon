namespace Beacon.API.Entities;

public class User
{
    public required Guid Id { get; init; }
    public required string EmailAddress { get; set; }
    public required string HashedPassword { get; set; }
}
