using Beacon.App.Entities;
using Beacon.App.ValueObjects;
using Beacon.Common.Memberships;
using Beacon.Common.Projects;

namespace Beacon.IntegrationTests.EndpointTests.Projects;

[Collection("ProjectTests")]
public class GetProjectsTests : EndpointTestBase
{
    public GetProjectsTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetProjects_ReturnsOnlyProjectsAssociatedWithCurrentLab()
    {
        AddTestAuthorization(LaboratoryMembershipType.Admin);

        var client = CreateClient(db =>
        {
            var lab = new Laboratory
            {
                Id = Guid.NewGuid(),
                Name = "Other Lab"
            };

            lab.AddMember(TestData.DefaultUser.Id, LaboratoryMembershipType.Admin);

            db.Laboratories.Add(lab);
            db.Projects.AddRange(new[]
            {
                new Project
                {
                    Id = Guid.NewGuid(),
                    CreatedById = TestData.DefaultUser.Id,
                    CustomerName = "ABC Customer",
                    ProjectCode = new ProjectCode("ABC", 1),
                    LaboratoryId = TestData.DefaultLaboratory.Id
                },
                new Project
                {
                    Id = Guid.NewGuid(),
                    CreatedById = TestData.DefaultUser.Id,
                    CustomerName = "123 Customer",
                    ProjectCode = new ProjectCode("123", 1),
                    LaboratoryId = lab.Id
                }
            });
            db.SaveChanges();
        });

        var uri = $"api/projects?laboratoryId={TestData.DefaultLaboratory.Id}";
        var projects = await client.GetFromJsonAsync<ProjectDto[]>(uri, JsonSerializerOptions);
        projects.Should().ContainSingle().Which.ProjectCode.Should().Be("ABC-001");
    }

}
