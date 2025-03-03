using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests;

public abstract class ProjectTestBase(TestFixture fixture) : IntegrationTestBase(fixture)
{
    protected static Guid ProjectId { get; } = new("a2871dc3-8746-45ad-bfd8-87e503d397cd");
    protected static ProjectCode ProjectCode { get; } = new("TST", "202301", 1);

    protected override IEnumerable<object> EnumerateReseedData()
    {
        yield return new Project
        {
            Id = ProjectId,
            ProjectCode = ProjectCode,
            CustomerName = "Test Customer",
            CreatedById = TestData.AdminUser.Id,
            LaboratoryId = TestData.Lab.Id
        };
    }
}
