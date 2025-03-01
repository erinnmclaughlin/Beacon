namespace Beacon.API.IntegrationTests.Endpoints;

public sealed class ApiFallbackTests(TestFixture fixture) : TestBase(fixture)
{
    [Fact]
    public async Task InvalidApiUrlReturnsNotFound()
    {
        var response = await HttpClient.GetAsync("api/not/real");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
