using Beacon.Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Beacon.WebApp.IntegrationTests.Http;

public static class MockHttpHelper
{
    public static MockHttpMessageHandler AddMockHttpClient(this TestServiceProvider services)
    {
        var mockHttpHandler = new MockHttpMessageHandler();

        services.AddSingleton(mockHttpHandler.ToHttpClientFactory());

        return mockHttpHandler;
    }

    public static IHttpClientFactory ToHttpClientFactory(this MockHttpMessageHandler mockHttpMessageHandler)
    {
        return new MockHttpClientFactory(mockHttpMessageHandler);
    }

    public static MockedRequest ThenRespondOK<T>(this MockedRequest request, T content)
    {
        return request.Respond(_ => CreateResponse(HttpStatusCode.OK, content));
    }

    public static MockedRequest ThenRespondNoContent(this MockedRequest request)
    {
        var response = new HttpResponseMessage(HttpStatusCode.NoContent);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return request.Respond(_ => response);
    }

    public static MockedRequest ThenRespondNotFound(this MockedRequest request)
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return request.Respond(_ => response);
    }

    public static MockedRequest ThenRespondValidationProblem(this MockedRequest request, Dictionary<string, string[]> errors)
    {
        return request.Respond(_ => CreateResponse(HttpStatusCode.UnprocessableEntity, new BeaconValidationProblem { Errors = errors }));
    }

    private static HttpResponseMessage CreateResponse<T>(HttpStatusCode statusCode, T content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(JsonSerializer.Serialize(content))
        };

        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return response;
    }
}
