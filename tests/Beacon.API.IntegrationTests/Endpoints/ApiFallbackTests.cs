namespace Beacon.API.IntegrationTests.Endpoints;

public sealed class ApiFallbackTests : TestBase
{
    public ApiFallbackTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task InvalidApiUrlReturnsNotFound()
    {
        var response = await _httpClient.GetAsync("api/not/real");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
