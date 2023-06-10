using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Http;

public class MockHttpClientFactory : IHttpClientFactory
{
    private readonly MockHttpMessageHandler _mockHttpMessageHandler;

    public MockHttpClientFactory(MockHttpMessageHandler mockHttpMessageHandler)
    {
        _mockHttpMessageHandler = mockHttpMessageHandler;
    }

    public HttpClient CreateClient(string name)
    {
        var httpClient = _mockHttpMessageHandler.ToHttpClient();
        httpClient.BaseAddress = new Uri("http://localhost");
        return httpClient;
    }
}