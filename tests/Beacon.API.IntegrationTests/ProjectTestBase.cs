using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests;

public abstract class ProjectTestBase : TestBase
{
    public static Guid ProjectId { get; } = new Guid("a2871dc3-8746-45ad-bfd8-87e503d397cd");
    public static ProjectCode ProjectCode { get; } = new("TST", "202301", 1);

    protected ProjectTestBase(TestFixture fixture) : base(fixture)
    {
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.Projects.Add(new Project
        {
            Id = ProjectId,
            ProjectCode = ProjectCode,
            CustomerName = "Test Customer",
            CreatedById = TestData.AdminUser.Id,
            LaboratoryId = TestData.Lab.Id
        });

        base.AddTestData(db);
    }

    protected Task<ProjectStatus> GetProjectStatusAsync() => ExecuteDbContextAsync(async db =>
    {
        return await db.Projects.Where(p => p.Id == ProjectId).Select(p => p.ProjectStatus).SingleAsync();
    });
}
