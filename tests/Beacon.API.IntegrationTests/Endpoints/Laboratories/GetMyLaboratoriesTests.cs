using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

[Trait("Feature", "Laboratory Management")]
public sealed class GetMyLaboratoriesTests : TestBase
{
    public static Guid OtherLabId { get; } = Guid.NewGuid();

    public GetMyLaboratoriesTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[185] Get my labs returns current user's labs only")]
    public async Task GetMyLaboratories_ReturnsOnlyCurrentUsersLabs()
    {
        RunAsManager();
        var response = await GetAsync("api/laboratories");
        var myLabs = await DeserializeAsync<LaboratoryDto[]>(response);

        Assert.NotNull(myLabs);
        Assert.Equal(2, myLabs.Length);
        Assert.Contains(myLabs, x => x.Name == "Some other lab");

        RunAsAdmin();
        var otherMyLabs = await DeserializeAsync<LaboratoryDto[]>(await GetAsync("api/laboratories"));

        Assert.NotNull(otherMyLabs);
        Assert.Single(otherMyLabs);
        Assert.DoesNotContain(otherMyLabs, x => x.Name == "Some other lab");
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        var lab = new Laboratory
        {
            Id = TestData.Lab.Id,
            Name = TestData.Lab.Name
        };

        lab.AddMember(TestData.AdminUser.Id, LaboratoryMembershipType.Admin);
        lab.AddMember(TestData.ManagerUser.Id, LaboratoryMembershipType.Manager);

        var otherLab = new Laboratory
        {
            Id = OtherLabId,
            Name = "Some other lab"
        };

        otherLab.AddMember(TestData.ManagerUser.Id, LaboratoryMembershipType.Admin);

        db.Users.AddRange(TestData.AdminUser, TestData.ManagerUser);
        db.Laboratories.AddRange(lab, otherLab);
        db.SaveChanges();
    }
}
