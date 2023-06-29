using Beacon.Common.Requests.Auth;
using Beacon.Common.Requests.Laboratories;
using System.Net;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

public sealed class CreateLaboratoryTests : TestBase
{
    public CreateLaboratoryTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Create lab succeeds when request is valid")]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "Test Lab"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact(DisplayName = "Create lab fails when request is invalid")]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }
}
