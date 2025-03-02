using Beacon.API.Persistence.Entities;
using Beacon.API.Services;
using Beacon.Common.Models;

namespace Beacon.WebHost;

public static class TestData
{
    public static User AdminUser => new()
    {
        Id = Guid.Parse("1d3685b6-14d9-4efd-90be-6e1c79277328"),
        DisplayName = "Admin",
        EmailAddress = "admin@test.test",
        HashedPassword = Hash("!!admin", out var salt),
        HashedPasswordSalt = salt
    };

    public static User ManagerUser => new()
    {
        Id = Guid.Parse("3754d2dc-0b97-4fc1-9e0e-ec65c9e58399"),
        DisplayName = "Manager",
        EmailAddress = "manager@test.test",
        HashedPassword = Hash("!!manager", out var salt),
        HashedPasswordSalt = salt
    };
    
    public static User AnalystUser => new()
    {
        Id = Guid.Parse("5b60079f-4cf3-4c45-aed8-6da525768f22"),
        DisplayName = "Analyst",
        EmailAddress = "analyst@test.test",
        HashedPassword = Hash("!!analyst", out var salt),
        HashedPasswordSalt = salt
    };

    public static User MemberUser => new()
    {
        Id = Guid.Parse("086e95fc-f132-482a-ba0c-6b082367766d"),
        DisplayName = "Member",
        EmailAddress = "member@test.test",
        HashedPassword = Hash("!!member", out var salt),
        HashedPasswordSalt = salt
    };

    public static User NonMemberUser => new()
    {
        Id = Guid.Parse("d0c19bae-f905-402b-9893-352d5550f05c"),
        DisplayName = "NonMember",
        EmailAddress = "notamember@test.test",
        HashedPassword = Hash("!!nonmember", out var salt),
        HashedPasswordSalt = salt
    };

    public static Laboratory Lab => new()
    {
        Id = Guid.Parse("0fa24c7c-eefb-4909-809d-4b14f0f6f247"),
        Name = "Test Lab"
    };

    private static readonly PasswordHasher PasswordHasher = new();
    private static string Hash(string password, out byte[] salt)
    {
        return PasswordHasher.Hash(password, out salt);
    }
}
