using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Projects;

public abstract class ProjectTestBase : TestBase
{
    public static Project Project => new()
    {
        Id = Guid.NewGuid(),
        CustomerName = "Test Customer",
        ProjectCode = new ProjectCode("TST", 1),
        CreatedById = TestData.AdminUser.Id,
        LaboratoryId = TestData.Lab.Id
    };

    protected ProjectTestBase(TestFixture fixture) : base(fixture)
    {
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.Projects.Add(Project);
        base.AddTestData(db);
        db.ChangeTracker.Clear();
    }
}
