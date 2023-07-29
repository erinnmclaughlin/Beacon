﻿using Beacon.Common.Models;
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
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [CascadingParameter]
    private BlazoredModalInstance Modal { get; set; } = default!;

    [Parameter]
    public Guid ProjectId { get; set; }

    private ErrorOr<LaboratoryInstrumentDto[]>? ErrorOrInstruments { get; set; }

    private CreateProjectEventRequest? _request;
    private CreateProjectEventRequest Request => _request ??= new()
    {
        ProjectId = ProjectId,
        ScheduledStart = DateTime.Today,
        ScheduledEnd = DateTime.Today.AddDays(1)
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

    private void DoSelectInstrument(LaboratoryInstrumentDto instrument)
    {
        if (Request.InstrumentIds.Contains(instrument.Id))
            Request.InstrumentIds.Remove(instrument.Id);
        else
            Request.InstrumentIds.Add(instrument.Id);
    }
}
