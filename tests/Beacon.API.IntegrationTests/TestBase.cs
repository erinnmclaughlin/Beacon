using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http.Json;

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

    protected void ResetDatabase()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        db.Reset();
    }

    protected void SetCurrentUser(Guid userId)
    {
        using var scope = _fixture.Services.CreateScope();
        var currentUserMock = scope.ServiceProvider.GetRequiredService<Mock<ICurrentUser>>();
        currentUserMock.SetupGet(x => x.UserId).Returns(userId);
    }

    protected async Task SendAsync(IRequest request)
    {
        using var scope = _fixture.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }

    protected async Task<T> SendAsync<T>(IRequest<T> request)
    {
        using var scope = _fixture.Services.CreateScope();
        return await scope.ServiceProvider.GetRequiredService<ISender>().Send(request);
    }

    protected async Task<HttpResponseMessage> PostAsync<T>(string uri, T? data)
    {
        return await _httpClient.PostAsJsonAsync(uri, data, JsonDefaults.JsonSerializerOptions);
    }

    protected async Task<HttpResponseMessage> PutAsync<T>(string uri, T? data)
    {
        return await _httpClient.PutAsJsonAsync(uri, data, JsonDefaults.JsonSerializerOptions);
    }

    protected async Task<HttpResponseMessage> GetAsync(string uri)
    {
        return await _httpClient.GetAsync(uri);
    }

    protected static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>(JsonDefaults.JsonSerializerOptions);
    }
}
