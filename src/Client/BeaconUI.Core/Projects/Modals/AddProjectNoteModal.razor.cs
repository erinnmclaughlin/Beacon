using Beacon.Common.Requests.Projects.Notes;
using BeaconUI.Core.Common.Forms;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Modals;

public sealed partial class AddProjectNoteModal
{
    private readonly IApiClient _apiClient;

    public AddProjectNoteModal(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = default!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    private AddProjectNoteRequest? _request;
    private AddProjectNoteRequest Request => _request ??= new()
    {
        ProjectId = ProjectId
    };

    private async Task Submit(BeaconForm formContext)
    {
        var result = await _apiClient.SendAsync(Request);
        if (result.IsError)
        {
            formContext.AddErrors(result.Errors);
            return;
        }

        await Modal.CloseAsync(ModalResult.Ok());
    }
}