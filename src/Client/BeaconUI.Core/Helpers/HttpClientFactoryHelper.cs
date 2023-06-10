namespace BeaconUI.Core.Helpers;
public static class HttpClientFactoryHelper
{
    public static HttpClient CreateBeaconClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient("BeaconApi");
    }
}
