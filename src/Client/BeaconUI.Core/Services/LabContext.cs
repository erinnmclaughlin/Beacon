using Beacon.Common.Models;
using Beacon.Common.Services;
using BeaconUI.Core.Clients;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Services;

public sealed class LabContext : ILabContext, IDisposable
{
    private readonly ApiClient _apiClient;
    private readonly ILocalStorageService _localStorage;

    public Guid LaboratoryId { get; private set; }
    public LaboratoryMembershipType? MembershipType { get; private set; }

    public LabContext(ApiClient apiClient, ILocalStorageService localStorage)
    {
        _apiClient = apiClient;
        _localStorage = localStorage;
        _localStorage.Changed += HandleChange;

        Initialize();
    }

    public void Dispose()
    {
        _localStorage.Changed -= HandleChange;
    }

    private async void Initialize()
    {
        LaboratoryId = await _localStorage.GetItemAsync<Guid>("CurrentLaboratoryId");

        if (LaboratoryId != Guid.Empty)
        {
            var lab = await _apiClient.GetCurrentLaboratory();
            MembershipType = lab.IsError ? null : lab.Value.MyMembershipType;
        }
    }

    private void HandleChange(object? o, ChangedEventArgs e)
    {
        if (e.Key != "CurrentLaboratoryId")
            return;

        LaboratoryId = e.NewValue is Guid id ? id : Guid.Empty;
    }
}
