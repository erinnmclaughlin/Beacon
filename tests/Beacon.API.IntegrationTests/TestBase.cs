using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests;
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
        _httpClient.Timeout = TimeSpan.FromMinutes(3);

        ResetDatabase();
    }

    protected virtual void AddTestData(BeaconDbContext db)
    {
        db.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser, TestData.NonMemberUser);
        db.Laboratories.Add(TestData.Lab);
        db.SaveChanges();
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

    protected void RunAsAdmin() => SetCurrentUser(TestData.AdminUser, LaboratoryMembershipType.Admin);
    protected void RunAsManager() => SetCurrentUser(TestData.ManagerUser, LaboratoryMembershipType.Manager);
    protected void RunAsAnalyst() => SetCurrentUser(TestData.AnalystUser, LaboratoryMembershipType.Analyst);
    protected void RunAsMember() => SetCurrentUser(TestData.MemberUser, LaboratoryMembershipType.Member);
    protected void RunAsNonMember() => SetCurrentUser(TestData.NonMemberUser, null);
    protected void RunAsAnonymous() => SetCurrentUser(null, null);

    protected void SetCurrentUser(User? user, LaboratoryMembershipType? membershipType)
    {
        using var scope = _fixture.Services.CreateScope();
        var sessionMock = scope.ServiceProvider.GetRequiredService<Mock<ISessionContext>>();
        sessionMock.SetupGet(x => x.UserId).Returns(user?.Id ?? Guid.Empty);
        sessionMock.SetupGet(x => x.CurrentUser).Returns(new CurrentUser
        {
            Id = user?.Id ?? Guid.Empty,
            DisplayName = user?.DisplayName ?? "",
        });
        sessionMock.SetupGet(x => x.CurrentLab).Returns(membershipType is null ? null : new CurrentLab
        {
            Id = TestData.Lab.Id,
            Name = TestData.Lab.Name,
            MembershipType = membershipType.Value
        });

    }

    protected Task<HttpResponseMessage> SendAsync<TRequest>(BeaconRequest<TRequest> request)
        where TRequest : BeaconRequest<TRequest>, IBeaconRequest<TRequest>, new()
    {
        return TRequest.SendAsync(_httpClient, request as TRequest ?? new());
    }

    protected Task<HttpResponseMessage> SendAsync<TRequest, TResponse>(BeaconRequest<TRequest, TResponse> request)
        where TRequest : BeaconRequest<TRequest, TResponse>, IBeaconRequest<TRequest>, new()
    {
        return TRequest.SendAsync(_httpClient, request as TRequest ?? new());
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
