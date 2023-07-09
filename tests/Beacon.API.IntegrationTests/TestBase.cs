using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests;

public abstract class TestBase : IClassFixture<TestFixture>
{
    protected readonly TestFixture _fixture;
    protected readonly HttpClient _httpClient;

    public TestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();

        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        ResetDatabase();
    }

    protected virtual void AddTestData(BeaconDbContext db)
    {
        AddDefaultTestData(db);
        db.SaveChanges();
    }
    
    protected void AddDefaultTestData(BeaconDbContext db)
    {
        db.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser, TestData.NonMemberUser);
        db.Laboratories.Add(TestData.Lab);
    }

    protected void ExecuteDbContext(Action<BeaconDbContext> action)
    {
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        action.Invoke(dbContext);
    }

    protected T ExecuteDbContext<T>(Func<BeaconDbContext, T> action)
    {
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        return action.Invoke(dbContext);
    }

    protected void RunAsAdmin() => SetCurrentUser(TestData.AdminUser.Id, LaboratoryMembershipType.Admin);
    protected void RunAsManager() => SetCurrentUser(TestData.ManagerUser.Id, LaboratoryMembershipType.Manager);
    protected void RunAsAnalyst() => SetCurrentUser(TestData.AnalystUser.Id, LaboratoryMembershipType.Analyst);
    protected void RunAsMember() => SetCurrentUser(TestData.MemberUser.Id, LaboratoryMembershipType.Member);
    protected void RunAsNonMember() => SetCurrentUser(TestData.NonMemberUser.Id, null);
    protected void RunAsAnonymous() => SetCurrentUser(Guid.Empty, null);

    protected void SetCurrentUser(Guid userId, LaboratoryMembershipType? membershipType)
    {
        using var scope = _fixture.Services.CreateScope();
        var currentUserMock = scope.ServiceProvider.GetRequiredService<Mock<ICurrentUser>>();
        currentUserMock.SetupGet(x => x.UserId).Returns(userId);
        currentUserMock.SetupGet(x => x.MembershipType).Returns(membershipType);
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

    private void ResetDatabase()
    {
        using var scope = _fixture.Services.CreateScope();
        var dbConnection = scope.ServiceProvider.GetRequiredService<DbConnection>();
        dbConnection.Close();

        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        dbContext.Database.EnsureDeleted();

        dbConnection.Open();
        dbContext.Database.EnsureCreated();
        AddTestData(dbContext);

        dbContext.ChangeTracker.Clear();
    }

}
