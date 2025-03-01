using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

[Trait("Feature", "Laboratory Management")]
public sealed class GetMyLaboratoriesTests(TestFixture fixture) : TestBase(fixture)
{
    private static Guid OtherLabId { get; } = Guid.NewGuid();

    [Fact(DisplayName = "[185] Get my labs returns current user's labs only")]
    public async Task GetMyLaboratories_ReturnsOnlyCurrentUsersLabs()
    {
        RunAsManager();
        var response = await SendAsync(new GetMyLaboratoriesRequest());
        var myLabs = await DeserializeAsync<LaboratoryDto[]>(response);

        Assert.NotNull(myLabs);
        Assert.Equal(2, myLabs.Length);
        Assert.Contains(myLabs, x => x.Name == "Some other lab");

        RunAsAdmin();
        var otherMyLabs = await DeserializeAsync<LaboratoryDto[]>(await SendAsync(new GetMyLaboratoriesRequest()));

        Assert.NotNull(otherMyLabs);
        Assert.Single(otherMyLabs);
        Assert.DoesNotContain(otherMyLabs, x => x.Name == "Some other lab");
    }

    protected override IEnumerable<object> EnumerateTestData()
    {
        foreach (var item in base.EnumerateTestData())
            yield return item;
        
        var otherLab = new Laboratory
        {
            Id = OtherLabId,
            Name = "Some other lab"
        };

        otherLab.AddMember(TestData.ManagerUser.Id, LaboratoryMembershipType.Admin);
        yield return otherLab;
    }
}
