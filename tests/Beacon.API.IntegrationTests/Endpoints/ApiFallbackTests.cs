using Microsoft.AspNetCore.TestHost;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "API")]
public sealed class ApiFallbackTests(WebApplicationFactory<Program> fixture) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task InvalidApiUrlReturnsNotFound()
    {
        var factory = fixture.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.ReplaceWithTestDatabase(null);
                services.UseFakeEmailService();
            });
        });
        
        using var httpClient = factory.CreateClient();
        
        var response = await httpClient.GetAsync("api/not/real", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
