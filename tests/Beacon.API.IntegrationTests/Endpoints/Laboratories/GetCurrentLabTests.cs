using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

[Trait("Feature", "Laboratory Management")]
public sealed class GetCurrentLabTests : TestBase
{
    public GetCurrentLabTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[185] Get current lab endpoint returns lab info if current user is a member")]
    public async Task GetCurrentLab_ReturnsExpectedResult()
    {
        RunAsAdmin();

        var response = await GetAsync("api/laboratories/current");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var lab = await DeserializeAsync<LaboratoryDto>(response);

        Assert.NotNull(lab);
        Assert.Equal(TestData.Lab.Id, lab.Id);
        Assert.Equal(TestData.Lab.Name, lab.Name);
    }

    [Fact(DisplayName = "[185] Get current lab endpoint does not return lab info if current user is not a member")]
    public async Task GetCurrentLab_ReturnsForbidden_IfCurrentUserIsNotAuthorized()
    {
        RunAsNonMember();

        var response = await GetAsync("api/laboratories/current");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        var lab = new Laboratory
        {
            Id = TestData.Lab.Id,
            Name = TestData.Lab.Name
        };

        lab.AddMember(TestData.AdminUser.Id, LaboratoryMembershipType.Admin);

        db.Users.Add(TestData.AdminUser);
        db.Laboratories.Add(lab);
        db.SaveChanges();
    }
}
