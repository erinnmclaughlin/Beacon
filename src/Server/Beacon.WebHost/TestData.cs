using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Memberships;
using Microsoft.EntityFrameworkCore;

namespace Beacon.WebHost;

internal static class TestData
{
    public static async Task InitializeForTests(this BeaconDbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        dbContext.Users.AddRange(AdminUser, ManagerUser, AnalystUser, MemberUser);
        await dbContext.SaveChangesAsync();

        dbContext.Laboratories.Add(Lab);
        await dbContext.SaveChangesAsync();
    }

    public static User AdminUser => new()
    {
        Id = Guid.Parse("1d3685b6-14d9-4efd-90be-6e1c79277328"),
        DisplayName = "Admin",
        EmailAddress = "admin@test.test",
        HashedPassword = new PasswordHasher().Hash("!!admin", out var salt),
        HashedPasswordSalt = salt
    };

    public static User ManagerUser => new()
    {
        Id = Guid.Parse("3754d2dc-0b97-4fc1-9e0e-ec65c9e58399"),
        DisplayName = "Manager",
        EmailAddress = "manager@test.test",
        HashedPassword = new PasswordHasher().Hash("!!manager", out var salt),
        HashedPasswordSalt = salt
    };

    public static User AnalystUser => new()
    {
        Id = Guid.Parse("5b60079f-4cf3-4c45-aed8-6da525768f22"),
        DisplayName = "Analyst",
        EmailAddress = "analyst@test.test",
        HashedPassword = new PasswordHasher().Hash("!!analyst", out var salt),
        HashedPasswordSalt = salt
    };

    public static User MemberUser => new()
    {
        Id = Guid.Parse("086e95fc-f132-482a-ba0c-6b082367766d"),
        DisplayName = "Member",
        EmailAddress = "member@test.test",
        HashedPassword = new PasswordHasher().Hash("!!member", out var salt),
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
            lab.AddMember(ManagerUser.Id, LaboratoryMembershipType.Manager);
            lab.AddMember(AnalystUser.Id, LaboratoryMembershipType.Analyst);
            lab.AddMember(MemberUser.Id, LaboratoryMembershipType.Member);
            return lab;
        }
    }


}
