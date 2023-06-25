using Blazored.LocalStorage;

namespace BeaconUI.Core.Services;

public sealed class CurrentLabService
{
    private readonly ILocalStorageService _localStorage;

    public CurrentLabService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SetCurrentLaboratory(Guid id)
    {
        await _localStorage.SetItemAsync("CurrentLaboratoryId", id);
    }

    public async Task<Guid?> GetCurrentLaboratoryId()
    {
        if (!await _localStorage.ContainKeyAsync("CurrentLaboratoryId"))
            return null;

        return await _localStorage.GetItemAsync<Guid>("CurrentLaboratoryId");
    }

    public async Task ClearCurrentLaboratory()
    {
        await _localStorage.RemoveItemAsync("CurrentLaboratoryId");
    }
}
