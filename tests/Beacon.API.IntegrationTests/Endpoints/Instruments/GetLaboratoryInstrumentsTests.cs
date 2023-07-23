using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;

namespace Beacon.API.IntegrationTests.Endpoints.Instruments;

public sealed class GetLaboratoryInstrumentsTests : TestBase
{
    public GetLaboratoryInstrumentsTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task GetLaboratoryInstruments_SucceedsWhenRequestIsValid()
    {
        RunAsAdmin();

        var response = await SendAsync(new GetLaboratoryInstrumentsRequest());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await DeserializeAsync<LaboratoryInstrumentDto[]>(response);
        Assert.NotNull(result);

        var instrument = Assert.Single(result);
        Assert.Equal("123", instrument.SerialNumber);
        Assert.Equal("Test", instrument.InstrumentType);
    }

    [Fact]
    public async Task GetProjectEvents_FailsWhenUserIsNotAuthorized()
    {
        RunAsNonMember();

        var response = await SendAsync(new GetLaboratoryInstrumentsRequest());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    protected override void AddTestData(BeaconDbContext db)
    {
        db.LaboratoryInstruments.Add(new LaboratoryInstrument
        {
            SerialNumber = "123",
            InstrumentType = "Test",
            LaboratoryId = TestData.Lab.Id
        });

        base.AddTestData(db);
    }
}
