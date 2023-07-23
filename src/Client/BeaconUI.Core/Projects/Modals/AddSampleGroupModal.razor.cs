using Beacon.Common.Requests.Projects.SampleGroups;
using BeaconUI.Core.Common.Forms;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Modals;

public partial class AddSampleGroupModal
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = default!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    private CreateSampleGroupRequest? _request;
    private CreateSampleGroupRequest Request => _request ??= new()
    {
        ProjectId = ProjectId
    };

    private async Task Submit(BeaconForm formContext)
    {
        var result = await ApiClient.SendAsync(Request);
        if (result.IsError)
        {
            formContext.AddErrors(result.Errors);
            return;
        }

        await Modal.CloseAsync(ModalResult.Ok());
    }
}