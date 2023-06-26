using BeaconUI.Core;

namespace Beacon.WebApp.IntegrationTests;

public abstract class BeaconTestContext : TestContext
{
    protected void SetupCoreServices()
    {
        Services.AddBeaconUI("localhost");
        this.AddBlazoredLocalStorage();
        JSInterop.SetupModule("./_content/Blazored.Modal/BlazoredModal.razor.js");
        JSInterop.SetupModule("https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js");
    }
}
