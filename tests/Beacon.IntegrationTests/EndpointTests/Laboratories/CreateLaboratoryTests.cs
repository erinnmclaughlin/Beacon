using Beacon.Common.Auth.Requests;
using Beacon.Common.Laboratories.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryTests : EndpointTestBase
{
    public CreateLaboratoryTests(BeaconTestApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        AddTestAuthorization();
        var response = await CreateClient().PostAsJsonAsync("portal/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        var client = CreateClient();
        await client.PostAsJsonAsync("portal/login", new LoginRequest
        {
            EmailAddress = TestData.DefaultUser.EmailAddress,
            Password = TestData.DefaultPassword
        });

        var response = await client.PostAsJsonAsync("portal/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "Test Lab"
        });

        response.EnsureSuccessStatusCode();
    }
}
