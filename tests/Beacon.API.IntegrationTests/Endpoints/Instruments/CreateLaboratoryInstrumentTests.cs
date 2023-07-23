using Beacon.Common.Requests.Instruments;

namespace Beacon.API.IntegrationTests.Endpoints.Instruments;

public sealed class CreateLaboratoryInstrumentTests : TestBase
{
    public CreateLaboratoryInstrumentTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
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

        ExecuteDbContext(dbContext =>
        {
            var instrument = dbContext.LaboratoryInstruments.Single();
            Assert.Equal(validRequest.SerialNumber, instrument.SerialNumber);
            Assert.Equal(validRequest.InstrumentType, instrument.InstrumentType);
            Assert.Equal(TestData.Lab.Id, instrument.LaboratoryId);
        });
    }

    [Fact]
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

        ExecuteDbContext(dbContext => Assert.Null(dbContext.LaboratoryInstruments.SingleOrDefault()));
    }

    [Fact]
    public async Task CreateProjectEvent_Fails_WhenRequestIsInvalid()
    {
        RunAsAdmin();

        var invalidRequest = new CreateLaboratoryInstrumentRequest
        {
            SerialNumber = "",
            InstrumentType = "FMS Headspace Analyzer"
        };

        var response = await SendAsync(invalidRequest);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);

        ExecuteDbContext(dbContext => Assert.Null(dbContext.ProjectEvents.SingleOrDefault()));
    }
}
