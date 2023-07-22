using BeaconUI.Core;
using BeaconUI.Core.Common.Auth;
using BeaconUI.Core.Common.Http;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Beacon.WebApp.IntegrationTests;

public abstract class BeaconTestContext : TestContext
{
    public BeaconTestContext()
    {
        Initialize();
    }

    protected void SetupCoreServices()
    {
        Services.AddBeaconUI("localhost");
        Services.RemoveAll<IApiClient>();
        Services.AddSingleton<Mock<IApiClient>>();
        Services.AddScoped(sp => sp.GetRequiredService<Mock<IApiClient>>().Object);

        this.AddBlazoredLocalStorage();
        JSInterop.SetupModule("./_content/Blazored.Modal/BlazoredModal.razor.js");
        JSInterop.SetupModule("https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js");
    }

    protected Mock<IApiClient> MockApi => Services.GetRequiredService<Mock<IApiClient>>();
    protected FakeNavigationManager NavigationManager => Services.GetRequiredService<FakeNavigationManager>();

    protected virtual void Initialize()
    {
        SetupCoreServices();
    }
}
