using Beacon.API.Persistence;
using Beacon.Common.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Beacon.API.IntegrationTests;

[Collection(nameof(TestFixture))]
public abstract class TestBase
{
    protected readonly TestFixture _fixture;
    protected readonly HttpClient _httpClient;

    public TestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        if (db.Database.EnsureCreated())
            db.AddTestData();

    }

    public void ResetDatabase()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        db.Reset();
    }

    public void SetCurrentUser(Guid userId)
    {
        using var scope = _fixture.Services.CreateScope();
        var currentUserMock = scope.ServiceProvider.GetRequiredService<Mock<ICurrentUser>>();
        currentUserMock.SetupGet(x => x.UserId).Returns(userId);
    }

    public async Task SendAsync(IRequest request)
    {
        using var scope = _fixture.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }

    public async Task<T> SendAsync<T>(IRequest<T> request)
    {
        using var scope = _fixture.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }
}
