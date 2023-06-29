using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

public sealed class GetMyLaboratoriesTests : TestBase
{
    public GetMyLaboratoriesTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Get my labs returns current user's labs only")]
    public async Task GetMyLaboratories_ReturnsOnlyCurrentUsersLabs()
    {
        SetCurrentUser(TestData.ManagerUser.Id);
        await PostAsync("api/laboratories", new CreateLaboratoryRequest { LaboratoryName = "Some other lab" });

        var response = await GetAsync("api/laboratories");
        var myLabs = await DeserializeAsync<LaboratoryDto[]>(response);

        Assert.NotNull(myLabs);
        Assert.Contains(myLabs, x => x.Name == "Some other lab");

        SetCurrentUser(TestData.AdminUser.Id);
        var otherMyLabs = await DeserializeAsync<LaboratoryDto[]>(await GetAsync("api/laboratories"));

        Assert.NotNull(otherMyLabs);
        Assert.DoesNotContain(otherMyLabs, x => x.Name == "Some other lab");
    }
}
