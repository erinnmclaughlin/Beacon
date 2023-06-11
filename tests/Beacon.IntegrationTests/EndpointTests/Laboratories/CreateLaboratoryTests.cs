using Beacon.Common.Laboratories.Requests;

namespace Beacon.IntegrationTests.EndpointTests.Laboratories;

public class CreateLaboratoryTests : EndpointTestBase
{
    private readonly HttpClient _httpClient;

    public CreateLaboratoryTests(BeaconTestApplicationFactory factory) : base(factory)
    {
        AddTestAuthorization();
        _httpClient = CreateClient();
    }

    [Fact]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        var response = await _httpClient.PostAsJsonAsync("portal/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        var response = await _httpClient.PostAsJsonAsync("portal/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "Test Lab"
        });

        response.EnsureSuccessStatusCode();
    }
}
