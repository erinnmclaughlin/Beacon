using Beacon.Common;
using Beacon.Common.Services;
using Beacon.Common.Validation;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using System.Net;
using System.Net.Http.Json;

namespace Beacon.WebApp.IntegrationTests.Common.Http;

public sealed class HttpResponseMessageExtensionsTests
{
    [Fact]
    public async Task HttpResponseMessageExtensions_ReturnsExpectedResult_WhenSuccessful()
    {
        var message = new HttpResponseMessage(HttpStatusCode.NoContent);
        var sut = await HttpResponseMessageExtensions.ToErrorOrResult(message);
        sut.IsError.Should().BeFalse();
        sut.Value.Should().BeEquivalentTo(Result.Success);
    }

    [Fact]
    public async Task HttpResponseMessageExtensions_ReturnsExpectedResult_WhenSuccessfulWithData()
    {
        var message = new HttpResponseMessage(HttpStatusCode.NoContent)
        {
            Content = JsonContent.Create(AuthHelper.DefaultSession, options: JsonDefaults.JsonSerializerOptions)
        };

        var sut = await HttpResponseMessageExtensions.ToErrorOrResult<SessionContext>(message);
        sut.IsError.Should().BeFalse();
        sut.Value.Should().BeEquivalentTo(AuthHelper.DefaultSession);
    }

    [Theory]
    [MemberData(nameof(TestData))]
    public async Task HttpResponseMessageExtensions_ReturnsExpectedResult_WhenError(HttpResponseMessage message, Error expectedError)
    {
        var sut = await HttpResponseMessageExtensions.ToErrorOrResult(message);
        sut.IsError.Should().BeTrue();
        sut.FirstError.Should().BeEquivalentTo(expectedError);
    }

    public static IEnumerable<object[]> TestData
    {
        get
        {
            yield return new object[] { CreateResponse(HttpStatusCode.NotFound), Error.NotFound() };
            yield return new object[] { CreateResponse(HttpStatusCode.UnprocessableEntity, new BeaconValidationProblem { Errors = new() { { "name", new[] { "value" } } } }), Error.Validation("name", "value") };
            yield return new object[] { CreateResponse(HttpStatusCode.Forbidden), Error.Failure() };
            yield return new object[] { CreateResponse(HttpStatusCode.Unauthorized), Error.Failure() };
        }
    }

    private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode)
    {
        return new HttpResponseMessage(statusCode);
    }

    private static HttpResponseMessage CreateResponse<T>(HttpStatusCode statusCode, T body)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = JsonContent.Create(body, options: JsonDefaults.JsonSerializerOptions)
        };
    }
}
