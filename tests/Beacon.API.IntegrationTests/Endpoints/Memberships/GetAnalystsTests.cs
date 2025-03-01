using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Memberships;

namespace Beacon.API.IntegrationTests.Endpoints.Memberships;

public sealed class GetAnalystsTests(TestFixture fixture) : ProjectTestBase(fixture)
{
    [Fact]
    public async Task SucceedsWhenRequestIsValid_ExcludeHistoricAnalysts()
    {
        RunAsAdmin();
        
        var response = await SendAsync(new GetAnalystsRequest { IncludeHistoricAnalysts = false });
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
        
        var response = await SendAsync(new GetAnalystsRequest { IncludeHistoricAnalysts = true });
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

    protected override IEnumerable<object> EnumerateTestData() => base.EnumerateTestData().Append(new Project
    {
        Id = Guid.NewGuid(),
        LaboratoryId = TestData.Lab.Id,
        CustomerName = "Test",
        ProjectCode = new ProjectCode("TST", "202105", 1),
        ProjectStatus = ProjectStatus.Completed,
        CreatedById = TestData.AdminUser.Id,
        LeadAnalystId = TestData.MemberUser.Id
    });
}
