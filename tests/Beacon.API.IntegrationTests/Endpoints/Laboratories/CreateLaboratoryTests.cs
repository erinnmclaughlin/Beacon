using Beacon.API.Persistence;
using Beacon.Common.Requests.Laboratories;

namespace Beacon.API.IntegrationTests.Endpoints.Laboratories;

public sealed class CreateLaboratoryTests : TestBase
{
    public CreateLaboratoryTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact(DisplayName = "Create lab succeeds when request is valid")]
    public async Task CreateLab_ShouldSucceed_WhenRequestIsValid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "Create Lab Test Name"
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var createdLab = ExecuteDbContext(db => db.Laboratories.SingleOrDefault(x => x.Name == "Create Lab Test Name"));
        Assert.NotNull(createdLab);
    }

    [Fact(DisplayName = "Create lab fails when request is invalid")]
    public async Task CreateLab_ShouldFail_WhenRequestIsInvalid()
    {
        SetCurrentUser(TestData.AdminUser.Id);

        var response = await PostAsync("api/laboratories", new CreateLaboratoryRequest
        {
            LaboratoryName = "no" // must be at least 3 characters
        });

        var createdLab = ExecuteDbContext(db => db.Laboratories.SingleOrDefault(x => x.Name == "Create Lab Test Name"));
        Assert.Null(createdLab);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.Users.Add(TestData.AdminUser);
        db.SaveChanges();
    }
}
