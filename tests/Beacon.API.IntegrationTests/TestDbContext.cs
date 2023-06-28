using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests;

public class TestDbContext : BeaconDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(new[]
        {
            TestData.AdminUser,
            TestData.ManagerUser,
            TestData.AnalystUser,
            TestData.MemberUser,
            TestData.NonMemberUser
        });

        modelBuilder.Entity<Laboratory>().HasData(new[]
        {
            new Laboratory { Id = TestData.Lab.Id, Name = TestData.Lab.Name }
        });

        modelBuilder.Entity<Membership>().HasData(new[]
        {
            new Membership 
            { 
                LaboratoryId = TestData.Lab.Id, 
                MemberId = TestData.AdminUser.Id, 
                MembershipType = LaboratoryMembershipType.Admin
            },
            new Membership
            {
                LaboratoryId = TestData.Lab.Id,
                MemberId = TestData.ManagerUser.Id,
                MembershipType = LaboratoryMembershipType.Manager
            },
            new Membership
            {
                LaboratoryId = TestData.Lab.Id,
                MemberId = TestData.AnalystUser.Id,
                MembershipType = LaboratoryMembershipType.Analyst
            },
            new Membership
            {
                LaboratoryId = TestData.Lab.Id,
                MemberId = TestData.MemberUser.Id,
                MembershipType = LaboratoryMembershipType.Member
            }
        });
    }
}
