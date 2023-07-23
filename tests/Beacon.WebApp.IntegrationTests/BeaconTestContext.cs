using BeaconUI.Core;
using BeaconUI.Core.Common.Http;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Beacon.WebApp.IntegrationTests;

public abstract class BeaconTestContext : TestContext
{
    public Mock<IApiClient> MockApi => Services.GetRequiredService<Mock<IApiClient>>();
    public FakeNavigationManager NavigationManager => Services.GetRequiredService<FakeNavigationManager>();

    public BeaconTestContext()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        // set up default beacon services:
        Services.AddBeaconUI("localhost"); 
        this.AddBlazoredLocalStorage();
        JSInterop.SetupModule("./_content/Blazored.Modal/BlazoredModal.razor.js");
        JSInterop.SetupModule("https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js");

        // mock IApiClient so we're not actually calling the API:
        Services.RemoveAll<IApiClient>();
        Services.AddSingleton<Mock<IApiClient>>();
        Services.AddScoped(sp => sp.GetRequiredService<Mock<IApiClient>>().Object);
    }

    protected void UrlShouldBe(string url)
    {
        NavigationManager.Uri.Should().Be(NavigationManager.BaseUri + url);
    }
}
