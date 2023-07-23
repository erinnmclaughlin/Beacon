using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Contacts;
using BeaconUI.Core.Common.Forms;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Modals;

public partial class UpdateContactModal
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = default!;

    [Parameter]
    public required Guid ProjectId { get; set; }

    [Parameter]
    public required ProjectContactDto ContactToUpdate { get; set; }

    private UpdateProjectContactRequest? _request;
    private UpdateProjectContactRequest Request => _request ??= new()
    {
        ProjectId = ProjectId,
        ContactId = ContactToUpdate.Id,
        EmailAddress = ContactToUpdate.EmailAddress,
        PhoneNumber = ContactToUpdate.PhoneNumber,
        Name = ContactToUpdate.Name
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