using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class GetAnalystsTests : ProjectTestBase
{
    public GetAnalystsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SucceedsWhenRequestIsValid_ExcludeHistoricAnalysts()
    {
        RunAsAdmin();
        var request = new GetAnalystsRequest { IncludeHistoricAnalysts = false };
        var response = await SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var members = await DeserializeAsync<LaboratoryMemberDto[]>(response);

        Assert.NotNull(members);
        Assert.Contains(members, m => m.Id == TestData.AdminUser.Id);
        Assert.Contains(members, m => m.Id == TestData.ManagerUser.Id);
        Assert.Contains(members, m => m.Id == TestData.AnalystUser.Id);
        Assert.DoesNotContain(members, m => m.Id == TestData.MemberUser.Id);
    }

    [Fact]
    public async Task SucceedsWhenRequestIsValid_IncludeHistoricAnalysts()
    {
        RunAsAdmin();
        var request = new GetAnalystsRequest { IncludeHistoricAnalysts = true };
        var response = await SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var members = await DeserializeAsync<LaboratoryMemberDto[]>(response);

        Assert.NotNull(members);
        Assert.Contains(members, m => m.Id == TestData.AdminUser.Id);
        Assert.Contains(members, m => m.Id == TestData.ManagerUser.Id);
        Assert.Contains(members, m => m.Id == TestData.AnalystUser.Id);
        Assert.Contains(members, m => m.Id == TestData.MemberUser.Id);
    }

    [Fact]
    public async Task FailsWhenUserIsNotAuthorized()
    {
        RunAsNonMember();
        var response = await SendAsync(new GetAnalystsRequest());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.Projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            LaboratoryId = TestData.Lab.Id,
            CustomerName = "Test",
            ProjectCode = new ProjectCode("TST", "202105", 1),
            ProjectStatus = ProjectStatus.Completed,
            CreatedById = TestData.AdminUser.Id,
            LeadAnalystId = TestData.MemberUser.Id
        });

        base.AddTestData(db);
    }
}
