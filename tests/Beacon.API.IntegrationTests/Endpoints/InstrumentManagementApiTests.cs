using System.Net.Http.Json;
using Beacon.API.Persistence.Entities;
using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.IntegrationTests.Endpoints;

[Trait("Category", "Instrument Management")]
public sealed class InstrumentManagementApiTests(TestFixture fixture) : IntegrationTestBase(fixture)
{
    /// <inheritdoc />
    protected override IEnumerable<object> EnumerateSeedData()
    {
        yield return TestData.AdminUser;
        yield return TestData.AnalystUser;
        yield return TestData.MemberUser;
        yield return TestData.NonMemberUser;
        yield return new LaboratoryInstrument
        {
            SerialNumber = "555",
            InstrumentType = "Test Type",
            LaboratoryId = TestData.Lab.Id
        };
    }
    
    [Fact(DisplayName = "[017] Adding an instrument to a laboratory succeeds when request is valid")]
    public async Task CreateLaboratoryInstrument_Succeeds_WhenRequestIsValid()
    {
        // Log in as a user that has permission to manage lab instruments:
        await LoginAndSetCurrentLab(TestData.AdminUser);
        
        // Attempt to create the instrument:
        var response = await HttpClient.SendAsync(new CreateLaboratoryInstrumentRequest
        {
            SerialNumber = "123",
            InstrumentType = "FMS Headspace Analyzer"
        });
        
        // Verify that this succeeds:
        response.EnsureSuccessStatusCode();

        // Verify that the persisted information matches the request:
        var instrument = await FindInstrument("123");
        Assert.NotNull(instrument);
        Assert.Equal("FMS Headspace Analyzer", instrument.InstrumentType);
        Assert.Equal(TestData.Lab.Id, instrument.LaboratoryId);
    }
    
    [Fact(DisplayName = "[017] Adding an instrument to a laboratory fails when user is not authorized")]
    public async Task CreateLaboratoryInstrument_Fails_WhenUserIsNotAuthorized()
    {
        // Log in as a user that does NOT have permission to manage lab instruments:
        await LoginAndSetCurrentLab(TestData.AnalystUser);

        // Attempt to create an instrument:
        var response = await HttpClient.SendAsync( new CreateLaboratoryInstrumentRequest
        {
            SerialNumber = "456",
            InstrumentType = "FMS Headspace Analyzer"
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Null(await FindInstrument("456"));
    }
    
    [Fact(DisplayName = "[017] Adding an instrument to a laboratory fails when request is invalid")]
    public async Task CreateLaboratoryInstrument_Fails_WhenRequestIsInvalid()
    {
        // Log in as a user that has permission to manage lab instruments:
        await LoginAndSetCurrentLab(TestData.AdminUser);

        // Attempt to create an instrument with invalid information:
        var response = await HttpClient.SendAsync(new CreateLaboratoryInstrumentRequest
        {
            SerialNumber = "789",
            InstrumentType = "" // required
        });
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Null(await FindInstrument("789"));
    }
    
    [Fact(DisplayName = "[017] Get lab instruments succeeds when request is valid")]
    public async Task GetLaboratoryInstruments_SucceedsWhenRequestIsValid()
    {
        // Log in as a user that has permission to view lab instruments:
        await LoginAndSetCurrentLab(TestData.MemberUser);
        
        // Attempt to get lab instruments:
        var response = await HttpClient.SendAsync(new GetLaboratoryInstrumentsRequest());
        
        // Verify that this succeeds:
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Verify that the response content contains the expected information about the instrument:
        var responseContent = await response.Content.ReadFromJsonAsync<LaboratoryInstrumentDto[]>();
        var instrument = responseContent?.SingleOrDefault(x => x.SerialNumber == "555");
        Assert.NotNull(instrument);
        Assert.Equal("Test Type", instrument.InstrumentType);
    }

    [Fact(DisplayName = "[017] Get lab instruments fails when user is not authorized")]
    public async Task GetProjectEvents_FailsWhenUserIsNotAuthorized()
    {
        // Log in as a user that does NOT have permission to view lab instruments:
        await LoginAs(TestData.NonMemberUser);

        // Attempt to get lab instruments:
        var response = await HttpClient.SendAsync(new GetLaboratoryInstrumentsRequest());
        
        // Verify that this fails:
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
    private Task<LaboratoryInstrument?> FindInstrument(string sn) => DbContext
        .Set<LaboratoryInstrument>()
        .IgnoreQueryFilters()
        .AsNoTracking()
        .SingleOrDefaultAsync(x => x.SerialNumber == sn);
}