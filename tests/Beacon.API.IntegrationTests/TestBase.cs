using Beacon.API.Persistence;
using Beacon.App.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace Beacon.API.IntegrationTests;

public abstract class TestBase : IClassFixture<ApiFactory>
{
    protected readonly ApiFactory _factory;
    protected readonly HttpClient _httpClient;

    public TestBase(ApiFactory factory)
    {
        _factory = factory;

        _httpClient = _factory.CreateClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        _httpClient.DefaultRequestHeaders.Add("X-LaboratoryId", TestData.Lab.Id.ToString());

        ResetState();
    }

    protected void SetCurrentUser(User? user)
    {
        if (user == null)
            _httpClient.DefaultRequestHeaders.Remove(TestAuthHandler.UserId);
        else
            _httpClient.DefaultRequestHeaders.Add(TestAuthHandler.UserId, user.Id.ToString());
    }

    protected void ResetState()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BeaconDbContext>();

        if (dbContext.Database.EnsureCreated())
        {
            SeedDatabase(dbContext);
            dbContext.SaveChanges();
        }

        dbContext.ChangeTracker.Clear();
    }

    protected virtual void SeedDatabase(BeaconDbContext dbContext)
    {
        dbContext.Users.AddRange(TestData.AdminUser, TestData.ManagerUser, TestData.AnalystUser, TestData.MemberUser, TestData.NonMemberUser);
        dbContext.Laboratories.Add(TestData.Lab);
    }
}
