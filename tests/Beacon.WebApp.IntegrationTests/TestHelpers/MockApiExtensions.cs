using Beacon.Common.Requests;
using BeaconUI.Core.Common.Http;
using ErrorOr;

namespace Beacon.WebApp.IntegrationTests.TestHelpers;

public static class MockApiExtensions
{
    public static Moq.Language.Flow.IReturnsResult<IApiClient> Succeeds<TRequest>(this Mock<IApiClient> apiMock) where TRequest : BeaconRequest<TRequest>, new()
    {
        return apiMock
            .Setup(x => x.SendAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);
    }

    public static Moq.Language.Flow.IReturnsResult<IApiClient> Succeeds<TRequest, TResponse>(this Mock<IApiClient> apiMock, TResponse result) where TRequest : BeaconRequest<TRequest, TResponse>, new()
    {
        return apiMock
            .Setup(x => x.SendAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
    }

    public static Moq.Language.Flow.IReturnsResult<IApiClient> Fails<TRequest>(this Mock<IApiClient> apiMock, Error? error = null) where TRequest : BeaconRequest<TRequest>, new()
    {
        return apiMock
            .Setup(x => x.SendAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(error ?? Error.Failure());
    }

    public static Moq.Language.Flow.IReturnsResult<IApiClient> Fails<TRequest, TResponse>(this Mock<IApiClient> apiMock, Error? error = null) where TRequest : BeaconRequest<TRequest, TResponse>, new()
    {
        return apiMock
            .Setup(x => x.SendAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(error ?? Error.Failure());
    }

}
