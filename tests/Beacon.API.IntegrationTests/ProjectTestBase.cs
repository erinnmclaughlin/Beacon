using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests;

[CollectionDefinition(nameof(ProjectTestCollection))]
public sealed class ProjectTestCollection : ICollectionFixture<TestFixture> { }

[Collection(nameof(ProjectTestCollection))]
public abstract class ProjectTestBase : TestBase
{
    public static Guid ProjectId { get; } = Guid.NewGuid();
    public static ProjectCode ProjectCode { get; } = new("TST", 1);

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
}
