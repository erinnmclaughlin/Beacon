using Beacon.Common.Requests.Projects.Contacts;
using BeaconUI.Core.Common.Forms;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Modals;

public partial class AddContactModal
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = default!;

    [Parameter]
    public required Guid ProjectId { get; set; }

    private CreateProjectContactRequest? _request;
    private CreateProjectContactRequest Request => _request ??= new()
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