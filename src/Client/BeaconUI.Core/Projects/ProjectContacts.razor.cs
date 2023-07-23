using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Contacts;
using BeaconUI.Core.Common.Http;
using BeaconUI.Core.Common.Modals;
using BeaconUI.Core.Projects.Modals;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects;

public partial class ProjectContacts
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [CascadingParameter]
    private IModalService ModalService { get; set; } = default!;

    [Parameter, EditorRequired]
    public required ProjectDto Project { get; set; }

    private ErrorOr<ProjectContactDto[]>? Contacts { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadProjectContacts();
    }

    private async Task LoadProjectContacts()
    {
        Contacts = await ApiClient.SendAsync(new GetProjectContactsRequest { ProjectId = Project.Id });
    }

    private async Task ShowAddContactModal()
    {
        var parameters = new ModalParameters().Add(nameof(AddContactModal.ProjectId), Project.Id);
        var result = await ModalService.Show<AddContactModal>("Add Contact", parameters).Result;

        if (result.Confirmed)
            await LoadProjectContacts();
    }

    private async Task ShowUpdateContactModal(ProjectContactDto contact)
    {
        var parameters = new ModalParameters().Add(nameof(UpdateContactModal.ProjectId), Project.Id).Add(nameof(UpdateContactModal.ContactToUpdate), contact);
        var result = await ModalService.Show<UpdateContactModal>("Update Contact", parameters).Result;
        
        if (result.Confirmed)
            await LoadProjectContacts();
    }

    private async Task ShowDeleteContactModal(ProjectContactDto contact)
    {
        var parameters = new ModalParameters().Add(nameof(AreYouSureModal.Content), $"Are you sure you want to remove {contact.Name} from project {Project.ProjectCode}?");
        var result = await ModalService.Show<AreYouSureModal>("Delete Contact", parameters).Result;
        
        if (result.Confirmed)
        {
            await ApiClient.SendAsync(new DeleteProjectContactRequest { ContactId = contact.Id, ProjectId = Project.Id });
            await LoadProjectContacts();
        }
    }
}