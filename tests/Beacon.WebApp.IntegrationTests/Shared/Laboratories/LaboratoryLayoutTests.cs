using AngleSharp.Dom;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Enums;
using Beacon.WebApp.IntegrationTests.Http;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

namespace Beacon.WebApp.IntegrationTests.Shared.Laboratories;

public class LaboratoryLayoutTests : BeaconTestContext
{
    [Fact]
    public void LabLayout_ShowsError_WhenApiReturnsError()
    {
        SetupCoreServices();
        Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

        var labId = Guid.NewGuid();

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, $"/api/laboratories/{labId}").ThenRespondNotFound();

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<BeaconUI.WebApp.App>();

        navManager.NavigateTo($"laboratories/{labId}");
        cut.WaitForState(() => cut.FindAll("h5").Any(m => m.GetInnerText() == "There was a problem loading lab details."));
    }

    [Fact]
    public void LabLayout_ShowsLabDetails_WhenApiReturnsOk()
    {
        SetupCoreServices();
        Services.AddScoped<IAuthorizationService, FakeAuthorizationService>();

        var lab = new LaboratoryDetailDto
        {
            Id = Guid.NewGuid(),
            Name = "Some Lab",
            CurrentUserMembershipType = LaboratoryMembershipType.Admin,
            Members = new List<LaboratoryMemberDto>()
        };

        var mockHttp = Services.AddMockHttpClient();
        mockHttp.When(HttpMethod.Get, $"/api/laboratories/{lab.Id}").ThenRespondOK(lab);

        var navManager = Services.GetRequiredService<FakeNavigationManager>();
        var cut = RenderComponent<BeaconUI.WebApp.App>();

        navManager.NavigateTo($"laboratories/{lab.Id}");
        cut.WaitForState(() => cut.Markup.Contains(lab.Name));
    }
}
