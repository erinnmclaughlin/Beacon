using Beacon.Common;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Services;

public sealed class CurrentLabService : ILabContext, IDisposable
{
    private readonly ILocalStorageService _localStorage;

    public Guid LaboratoryId { get; private set; }

    public CurrentLabService(ILocalStorageService localStorage)
    {
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
        var task = _localStorage.GetItemAsync<Guid>("CurrentLaboratoryId");

        while (!task.IsCompleted) 
        {
            await Task.Delay(200);
        }

        LaboratoryId = task.Result;
    }

    private void HandleChange(object? o, ChangedEventArgs e)
    {
        if (e.Key != "CurrentLaboratoryId")
            return;

        LaboratoryId = e.NewValue is Guid id ? id : Guid.Empty;
    }
}
