using BeaconUI.Core;
using BeaconUI.WebApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("BeaconApi", options =>
{
    options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddBeaconUI();

await builder.Build().RunAsync();
