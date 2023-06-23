using Beacon.Common.Laboratories;
using BeaconUI.Core.Clients;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Services;

public sealed class CurrentLabService
{
    private readonly ApiClient _apiClient;
    private readonly ILocalStorageService _localStorage;

    public CurrentLabService(ApiClient apiClient, ILocalStorageService localStorage)
    {
        _apiClient = apiClient;
        _localStorage = localStorage;
    }

    public async Task SetCurrentLaboratory(Guid id)
    {
        await _localStorage.SetItemAsync("CurrentLaboratoryId", id);
    }

    public async Task<LaboratoryDto?> GetCurrentLaboratory()
    {
        if (!await _localStorage.ContainKeyAsync("CurrentLaboratoryId"))
            return null;

        var labId = await _localStorage.GetItemAsync<Guid>("CurrentLaboratoryId");
        var errorOrLab = await _apiClient.GetLaboratoryById(labId);
        return errorOrLab.IsError ? null : errorOrLab.Value;
    }

    public async Task ClearCurrentLaboratory()
    {
        await _localStorage.RemoveItemAsync("CurrentLaboratoryId");
    }
}
