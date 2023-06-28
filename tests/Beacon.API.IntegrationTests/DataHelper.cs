using Beacon.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests;

public static class DataHelper
{
    public static void AddTestData(this BeaconDbContext db)
    {
        db.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser, TestData.NonMemberUser);
        db.Laboratories.Add(TestData.Lab);
        db.SaveChanges();
    }

    public static void DeleteTestData(this BeaconDbContext db)
    {
        db.Memberships.ExecuteDelete();
        db.Users.ExecuteDelete();
        db.Laboratories.ExecuteDelete();
    }

    public static void Reset(this BeaconDbContext db)
    {
        db.DeleteTestData();
        db.AddTestData();
    }
}
