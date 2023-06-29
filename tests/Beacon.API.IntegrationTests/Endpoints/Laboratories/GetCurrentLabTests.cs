using Beacon.API.Persistence;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

public sealed class GetCurrentLabTests : CoreTestBase
{
    public GetCurrentLabTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetCurrentLab_ReturnsExpectedResult()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await GetAsync("api/laboratories/current");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var lab = await DeserializeAsync<LaboratoryDto>(response);

        Assert.NotNull(lab);
        Assert.Equal(TestData.Lab.Id, lab.Id);
        Assert.Equal(TestData.Lab.Name, lab.Name);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        var lab = new App.Entities.Laboratory
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
