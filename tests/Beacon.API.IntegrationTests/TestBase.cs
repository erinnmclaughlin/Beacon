using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests;
using Beacon.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Beacon.API.IntegrationTests;

[CollectionDefinition("TestCollection")]
public class TestCollection : ICollectionFixture<TestFixture>
{
}

[Collection("TestCollection")]
public abstract class TestBase : IAsyncLifetime
{
    protected readonly TestFixture _fixture;
    protected readonly HttpClient _httpClient;

    protected TestBase(TestFixture fixture)
    {
        _fixture = fixture;
        _httpClient = _fixture.CreateClient();
        _httpClient.Timeout = TimeSpan.FromMinutes(3);
    }

    public async Task InitializeAsync()
    {
        await ResetDatabase();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual void AddTestData(BeaconDbContext db)
    {
        db.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser, TestData.NonMemberUser);
        db.Laboratories.Add(TestData.Lab);
    }
    
    protected async Task ExecuteDbContextAsync(Func<BeaconDbContext, Task> action)
    {
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        await action.Invoke(dbContext);
    }

    protected async Task<T> ExecuteDbContextAsync<T>(Func<BeaconDbContext, Task<T>> action)
    {
        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        return await action.Invoke(dbContext);
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

    private async Task ResetDatabase()
    {
        await _fixture.ResetDatabase();

        using var scope = _fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        AddTestData(dbContext);
        await dbContext.SaveChangesAsync();

        dbContext.ChangeTracker.Clear();
    }
}
