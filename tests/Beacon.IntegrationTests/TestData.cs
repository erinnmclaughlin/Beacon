using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.App.Entities;
using Beacon.Common.Memberships;

namespace Beacon.IntegrationTests;

public static class TestData
{
    public static string DefaultPassword { get; } = "password123";

    public static User DefaultUser => new User
    {
        Id = new Guid("3e8d5902-9574-450a-8f23-7243a82795e9"),
        DisplayName = "Test",
        EmailAddress = "test@test.com",
        HashedPassword = new PasswordHasher().Hash(DefaultPassword, out var salt),
        HashedPasswordSalt = salt
    };

    public static Laboratory DefaultLaboratory => new Laboratory 
    {
        Id = new Guid("8b4f86b7-95fc-401c-a322-202e8d1ab0c3"),
        Name = "Test Lab"
    };

    public static void SeedWithTestData(this BeaconDbContext dbContext, LaboratoryMembershipType? membershipType)
    {
        dbContext.Users.Add(DefaultUser);
        dbContext.Laboratories.Add(DefaultLaboratory);

        if (membershipType is not null)
        {
            dbContext.Memberships.Add(new Membership
            {
                LaboratoryId = DefaultLaboratory.Id,
                MemberId = DefaultUser.Id,
                MembershipType = membershipType.Value
            });
        }

        dbContext.SaveChanges();
    }
}
