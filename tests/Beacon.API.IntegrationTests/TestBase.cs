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
    protected TestFixture Fixture { get; }
    protected HttpClient HttpClient { get; }

    protected TestBase(TestFixture fixture)
    {
        Fixture = fixture;
        HttpClient = Fixture.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await Fixture.ResetDatabase();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual IEnumerable<object> EnumerateTestData()
    {
        yield return TestData.AdminUser;
        yield return TestData.ManagerUser;
        yield return TestData.AnalystUser;
        yield return TestData.MemberUser;
        yield return TestData.NonMemberUser;
        yield return TestData.Lab;
    }
    
    protected async Task ExecuteDbContextAsync(Func<BeaconDbContext, Task> action)
    {
        using var scope = Fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        await action.Invoke(dbContext);
    }

    protected async Task<T> ExecuteDbContextAsync<T>(Func<BeaconDbContext, Task<T>> action)
    {
        using var scope = Fixture.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();
        return await action.Invoke(dbContext);
    }

    protected void RunAsAdmin() => SetCurrentUser(TestData.AdminUser, LaboratoryMembershipType.Admin);
    protected void RunAsManager() => SetCurrentUser(TestData.ManagerUser, LaboratoryMembershipType.Manager);
    protected void RunAsAnalyst() => SetCurrentUser(TestData.AnalystUser, LaboratoryMembershipType.Analyst);
    protected void RunAsMember() => SetCurrentUser(TestData.MemberUser, LaboratoryMembershipType.Member);
    protected void RunAsNonMember() => SetCurrentUser(TestData.NonMemberUser);
    protected void RunAsAnonymous() => SetCurrentUser(null);

    protected void SetCurrentUser(User? user, LaboratoryMembershipType? membershipType = null)
    {
        using var scope = Fixture.Services.CreateScope();
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
        return TRequest.SendAsync(HttpClient, request as TRequest ?? new());
    }

    protected Task<HttpResponseMessage> SendAsync<TRequest, TResponse>(BeaconRequest<TRequest, TResponse> request)
        where TRequest : BeaconRequest<TRequest, TResponse>, IBeaconRequest<TRequest>, new()
    {
        return TRequest.SendAsync(HttpClient, request as TRequest ?? new());
    }

    protected static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>(JsonDefaults.JsonSerializerOptions);
    }
}
