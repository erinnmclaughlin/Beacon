using Beacon.Common;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Services;

public sealed class LabContext : ILabContext, IDisposable
{
    private readonly ILocalStorageService _localStorage;

    public Guid LaboratoryId { get; private set; }

    public LabContext(ILocalStorageService localStorage)
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

        var count = 0;
        while (!task.IsCompleted) 
        {
            await Task.Delay(200);

            // allow max of roughly 2 seconds to initialize:
            if (count++ >= 10)
                throw new TimeoutException("Unable to initialize lab context. Local storage call has timed out.");
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
