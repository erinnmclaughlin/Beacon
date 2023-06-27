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
    }

    private void HandleChange(object? o, ChangedEventArgs e)
    {
        if (e.Key != "CurrentLaboratoryId")
            return;

        LaboratoryId = e.NewValue is Guid id ? id : Guid.Empty;
    }

    public async Task<LaboratoryMembershipType?> GetMembershipTypeAsync(Guid userId, CancellationToken ct = default)
    {
        if (LaboratoryId == Guid.Empty)
            return null;

        var membershipOrError = await _apiClient.GetLaboratoryMembers();

        return membershipOrError.Match(
            members => members.FirstOrDefault(m => m.Id == userId)?.MembershipType,
            error => null
        );
    }
}
