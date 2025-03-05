using Beacon.Common;
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
        Request.ScheduledStart = datePart.ToDateTimeOffset(Request.ScheduledStart.ToTimeOnly());
    }

    private void UpdateStartTime(TimeOnly timePart)
    {
        Request.ScheduledStart = Request.ScheduledStart.WithNewTimePart(timePart);
    }

    private void UpdateEndDate(DateOnly datePart)
    {
        Request.ScheduledEnd = datePart.ToDateTimeOffset(Request.ScheduledEnd.ToTimeOnly());
    }

    private void UpdateEndTime(TimeOnly timePart)
    {
        Request.ScheduledEnd = Request.ScheduledEnd.WithNewTimePart(timePart);
    }
}
