using Beacon.Common.Requests.Laboratories;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

[Trait("Feature", "Laboratory Management")]
public sealed class CreateLaboratoryTests : TestBase
{
    public CreateLaboratoryTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "[002] Create lab succeeds when request is valid")]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateLaboratoryRequest
        {
            LaboratoryName = "My New Lab"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(await LabExists("My New Lab"));
    }

    [Fact(DisplayName = "[002] Create lab fails when request is invalid")]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var response = await SendAsync(new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.False(await LabExists("no"));
    }

    private Task<bool> LabExists(string labName) => ExecuteDbContextAsync(async db =>
    {
        return await db.Laboratories.AnyAsync(x => x.Name == labName);
    });
}
