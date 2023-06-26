namespace Beacon.API.IntegrationTests.Endpoints.Members;

[Collection(MembersTests.Name)]
public class GetLaboratoryMembersTests : TestBase
{
    public GetLaboratoryMembersTests(DbContextFixture db, WebApplicationFactory<BeaconWebHost> factory) : base(db, factory)
    {
    }
}
