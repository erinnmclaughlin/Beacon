using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;
using Beacon.Common.Requests.Projects.Events;
using BeaconUI.Core.Common.Forms;
using BeaconUI.Core.Common.Http;
using Blazored.Modal;
using Blazored.Modal.Services;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Modals;

public partial class CreateProjectEventModal
{
    public static IModalReference Show(IModalService modalService, Guid projectId)
    {
        var parameters = new ModalParameters().Add(nameof(ProjectId), projectId);
        return modalService.Show<CreateProjectEventModal>("Schedule an Event", parameters);
    }

    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = null!;

    [Parameter]
    public Guid ProjectId { get; set; }

    private ErrorOr<LaboratoryInstrumentDto[]>? ErrorOrInstruments { get; set; }

    private CreateProjectEventRequest? _request;
    private CreateProjectEventRequest Request => _request ??= new()
    {
        ProjectId = ProjectId,
        ScheduledStart = DateTimeOffset.UtcNow.Date,
        ScheduledEnd = DateTimeOffset.UtcNow.Date.AddDays(1)
    };

    protected override async Task OnInitializedAsync()
    {
        ErrorOrInstruments = await ApiClient.SendAsync(new GetLaboratoryInstrumentsRequest());
    }

    private async Task Submit(BeaconForm formContext)
    {
        await ApiClient.SendAsync(Request);
        await Modal.CloseAsync(ModalResult.Ok());
    }

    private void UpdateStartDate(DateOnly datePart)
    {
        var timePart = TimeOnly.FromDateTime(Request.ScheduledStart.DateTime);
        Request.ScheduledStart = datePart.ToDateTime(timePart, DateTimeKind.Utc);
    }

    private void UpdateStartTime(TimeOnly timePart)
    {
        var datePart = DateOnly.FromDateTime(Request.ScheduledStart.DateTime);
        Request.ScheduledStart = datePart.ToDateTime(timePart, DateTimeKind.Utc);
    }

    private void UpdateEndDate(DateOnly datePart)
    {
        var timePart = TimeOnly.FromDateTime(Request.ScheduledEnd.DateTime);
        Request.ScheduledEnd = datePart.ToDateTime(timePart, DateTimeKind.Utc);
    }

    private void UpdateEndTime(TimeOnly timePart)
    {
        var datePart = DateOnly.FromDateTime(Request.ScheduledEnd.DateTime);
        Request.ScheduledEnd = datePart.ToDateTime(timePart, DateTimeKind.Utc);
    }
}
