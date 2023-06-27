using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Models;

namespace Beacon.WebHost;

public static class TestData
{
    public static async Task InitializeForTests(this BeaconDbContext dbContext)
    {
        if (await dbContext.Database.EnsureCreatedAsync())
        {
            dbContext.Users.AddRange(AdminUser, ManagerUser, AnalystUser, MemberUser);
            dbContext.Laboratories.Add(Lab);
            await dbContext.SaveChangesAsync();
        }
    }

    public static User AdminUser => new()
    {
        Id = Guid.Parse("1d3685b6-14d9-4efd-90be-6e1c79277328"),
        DisplayName = "Admin",
        EmailAddress = "admin@test.test",
        HashedPassword = Hash("!!admin", out var salt),
        HashedPasswordSalt = salt
    };

    public static User AdminUserAlt => new()
    {
        Id = Guid.Parse("20a26392-3933-4b4d-a884-c83338aaca98"),
        DisplayName = "Admin Alt",
        EmailAddress = "adminalt@test.test",
        HashedPassword = Hash("!!adminalt", out var salt),
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
    
    public static User ManagerUserAlt => new()
    {
        Id = Guid.Parse("89500450-4cb7-4e32-935f-121c3aa0ff90"),
        DisplayName = "Manager Alt",
        EmailAddress = "manageralt@test.test",
        HashedPassword = Hash("!!manageralt", out var salt),
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


    public static User AnalystUserAlt => new()
    {
        Id = Guid.Parse("e64e53aa-60e2-404a-a0b8-c1dfe135aca7"),
        DisplayName = "Analyst Alt",
        EmailAddress = "analystalt@test.test",
        HashedPassword = Hash("!!analystalt", out var salt),
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

    public static User MemberUserAlt => new()
    {
        Id = Guid.Parse("aa4c47b2-56c2-49b2-8cb9-0727169e4acc"),
        DisplayName = "Member Alt",
        EmailAddress = "memberalt@test.test",
        HashedPassword = Hash("!!memberalt", out var salt),
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

    public static Laboratory Lab
    {
        get
        {
            var lab = new Laboratory
            {
                Id = Guid.Parse("0fa24c7c-eefb-4909-809d-4b14f0f6f247"),
                Name = "Test Lab"
            };

            lab.AddMember(AdminUser.Id, LaboratoryMembershipType.Admin);
            lab.AddMember(AdminUserAlt.Id, LaboratoryMembershipType.Admin);
            lab.AddMember(ManagerUser.Id, LaboratoryMembershipType.Manager);
            lab.AddMember(ManagerUserAlt.Id, LaboratoryMembershipType.Manager);
            lab.AddMember(AnalystUser.Id, LaboratoryMembershipType.Analyst);
            lab.AddMember(AnalystUserAlt.Id, LaboratoryMembershipType.Analyst);
            lab.AddMember(MemberUser.Id, LaboratoryMembershipType.Member);
            lab.AddMember(MemberUserAlt.Id, LaboratoryMembershipType.Member);
            return lab;
        }
    }

    private static readonly PasswordHasher _passwordHasher = new();
    private static string Hash(string password, out byte[] salt)
    {
        return _passwordHasher.Hash(password, out salt);
    }
}
