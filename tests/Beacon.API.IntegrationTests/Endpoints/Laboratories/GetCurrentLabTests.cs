using Beacon.Common.Models;
using System.Net;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

public sealed class GetCurrentLabTests : TestBase
{
    public GetCurrentLabTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetCurrentLab_ReturnsExpectedResult()
    {
        SetCurrentUser(TestData.MemberUser.Id);

        var response = await GetAsync("api/laboratories/current");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var lab = await DeserializeAsync<LaboratoryDto>(response);

        Assert.NotNull(lab);
        Assert.Equal(TestData.Lab.Id, lab.Id);
        Assert.Equal(TestData.Lab.Name, lab.Name);
    }
}
