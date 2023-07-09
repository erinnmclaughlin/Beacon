using Blazored.LocalStorage;

namespace BeaconUI.Core.Common.Http;

public class BeaconHttpHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public BeaconHttpHeaderHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var labId = await _localStorage.GetItemAsync<Guid>("CurrentLaboratoryId", ct);

        if (labId != Guid.Empty)
            request.Headers.Add("X-LaboratoryId", labId.ToString());

        return await base.SendAsync(request, ct);
    }
}
