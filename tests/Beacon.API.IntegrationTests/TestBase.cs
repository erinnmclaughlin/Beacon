using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
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
            AddTestData(db);
    }

    protected virtual void AddTestData(BeaconDbContext db)
    {
        db.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser, TestData.NonMemberUser);
        db.Laboratories.Add(TestData.Lab);
        db.SaveChanges();
    }
    
    protected void ResetDatabase()
    {
        using var scope = _fixture.Services.CreateScope();
        var dbConnection = scope.ServiceProvider.GetRequiredService<DbConnection>();
        dbConnection.Close();

        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        dbContext.Database.EnsureDeleted();

        dbConnection.Open();
    }

    protected void SetCurrentUser(Guid userId)
    {
        using var scope = _fixture.Services.CreateScope();
        var currentUserMock = scope.ServiceProvider.GetRequiredService<Mock<ICurrentUser>>();
        currentUserMock.SetupGet(x => x.UserId).Returns(userId);
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
