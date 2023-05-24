using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace BeaconUI.Core.Auth;

public class CookieHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await base.SendAsync(request, ct);
    }
}
