using Beacon.Common.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Beacon.API.IntegrationTests;

[Collection(nameof(TestFixture))]
public abstract class TestBase
{
    private readonly IServiceScope _scope;

    public TestBase()
    {
        _scope = TestFixture.BaseScopeFactory.CreateScope();
    }

    public void SetCurrentUser(Guid userId)
    {
        var currentUserMock = _scope.ServiceProvider.GetRequiredService<Mock<ICurrentUser>>();
        currentUserMock.SetupGet(x => x.UserId).Returns(userId);
    }

    public async Task SendAsync(IRequest request)
    {
        await _scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }

    public async Task<T> SendAsync<T>(IRequest<T> request)
    {
        return await _scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }
}
