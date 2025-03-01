using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

[Trait("Feature", "Laboratory Management")]
public sealed class GetCurrentLabTests(TestFixture fixture) : TestBase(fixture)
{
    [Fact(DisplayName = "[185] Get current lab endpoint returns lab info if current user is a member")]
    public async Task GetCurrentLab_ReturnsExpectedResult()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetCurrentLaboratoryRequest());
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

        var response = await SendAsync(new GetCurrentLaboratoryRequest());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
