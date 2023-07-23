using Beacon.Common.Requests;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using System.Linq.Expressions;

namespace Beacon.WebApp.IntegrationTests.TestHelpers;

public static class MockApiExtensions
{
    public static Moq.Language.Flow.ISetup<IApiClient, Task<ErrorOr<Success>>> Setup<TRequest>(this Mock<IApiClient> apiMock) where TRequest : BeaconRequest<TRequest>, new()
    {
        return apiMock.Setup(GetSetupExpression<TRequest>());
    }

    public static Moq.Language.Flow.ISetup<IApiClient, Task<ErrorOr<TResponse>>> Setup<TRequest, TResponse>(this Mock<IApiClient> apiMock) where TRequest : BeaconRequest<TRequest, TResponse>, new()
    {
        return apiMock.Setup(GetSetupExpression<TRequest, TResponse>());
    }

    public static void Verify<TRequest>(this Mock<IApiClient> apiMock, Times times) where TRequest : BeaconRequest<TRequest>, new()
    {
        apiMock.Verify(GetSetupExpression<TRequest>(), times);
    }

    public static void Verify<TRequest, TResponse>(this Mock<IApiClient> apiMock, Times times) where TRequest : BeaconRequest<TRequest, TResponse>, new()
    {
        apiMock.Verify(GetSetupExpression<TRequest, TResponse>(), times);
    }

    public static Moq.Language.Flow.IReturnsResult<IApiClient> Succeeds<TRequest>(this Mock<IApiClient> apiMock) where TRequest : BeaconRequest<TRequest>, new()
    {
        return apiMock.Setup<TRequest>().ReturnsAsync(Result.Success);
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

    private static Expression<Func<IApiClient, Task<ErrorOr<Success>>>> GetSetupExpression<TRequest>() where TRequest : BeaconRequest<TRequest>, new()
    {
        return x => x.SendAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>());
    }

    private static Expression<Func<IApiClient, Task<ErrorOr<TResponse>>>> GetSetupExpression<TRequest, TResponse>() where TRequest : BeaconRequest<TRequest, TResponse>, new()
    {
        return x => x.SendAsync(It.IsAny<TRequest>(), It.IsAny<CancellationToken>());
    }


}
