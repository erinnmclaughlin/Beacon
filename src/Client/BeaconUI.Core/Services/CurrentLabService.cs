using Beacon.Common;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Services;

public sealed class CurrentLabService : ILabContext
{
    private readonly ILocalStorageService _localStorage;

    public CurrentLabService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<Guid> GetLaboratoryId()
    {
        return await _localStorage.GetItemAsync<Guid>("CurrentLaboratoryId");
    }

    public async Task SetLaboratoryId(Guid id)
    {
        await _localStorage.SetItemAsync("CurrentLaboratoryId", id);
    }
}
