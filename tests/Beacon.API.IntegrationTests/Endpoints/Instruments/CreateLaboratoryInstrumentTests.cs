using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Instruments;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints.Instruments;

[Trait("Feature", "Instrument Management")]
public sealed class CreateLaboratoryInstrumentTests(TestFixture fixture) : TestBase(fixture)
{
    [Fact(DisplayName = "[017] Adding an instrument to a laboratory succeeds when request is valid")]
    public async Task CreateLaboratoryInstrument_Succeeds_WhenRequestIsValid()
    {
        RunAsAdmin();

        var validRequest = new CreateLaboratoryInstrumentRequest
        {
            SerialNumber = "123",
            InstrumentType = "FMS Headspace Analyzer"
        };

        var response = await SendAsync(validRequest);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var instrument = await GetInstrumentAsync();
        Assert.NotNull(instrument);
        Assert.Equal(validRequest.SerialNumber, instrument.SerialNumber);
        Assert.Equal(validRequest.InstrumentType, instrument.InstrumentType);
        Assert.Equal(TestData.Lab.Id, instrument.LaboratoryId);
    }

    [Fact(DisplayName = "[017] Adding an instrument to a laboratory fails when user is not authorized")]
    public async Task CreateLaboratoryInstrument_Fails_WhenUserIsNotAuthorized()
    {
        RunAsAnalyst();

        var validRequest = new CreateLaboratoryInstrumentRequest
        {
            SerialNumber = "123",
            InstrumentType = "FMS Headspace Analyzer"
        };

        var response = await SendAsync(validRequest);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var instrument = await GetInstrumentAsync();
        Assert.Null(instrument);
    }

    [Fact(DisplayName = "[017] Adding an instrument to a laboratory fails when request is invalid")]
    public async Task CreateLaboratoryInstrument_Fails_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var invalidRequest = new CreateLaboratoryInstrumentRequest
        {
            SerialNumber = "",
            InstrumentType = "FMS Headspace Analyzer"
        };

        var response = await SendAsync(invalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        var instrument = await GetInstrumentAsync();
        Assert.Null(instrument);
    }

    private async Task<LaboratoryInstrument?> GetInstrumentAsync()
    {
        return await ExecuteDbContextAsync(async db => await db.LaboratoryInstruments.SingleOrDefaultAsync());
    }
    
}
