﻿using BeaconUI.Core.Projects.Modals;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Components;

public partial class CreateProjectEventButton
{
    [CascadingParameter]
    private IModalService ModalService { get; set; } = default!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    [Parameter]
    public EventCallback OnEventCreated { get; set; }

    private async Task Click()
    {
        var modalParameters = new ModalParameters()
            .Add(nameof(CreateProjectEventModal.ProjectId), ProjectId);

        var result = await ModalService
            .Show<CreateProjectEventModal>("Create New Project", modalParameters)
            .Result;

        if (!result.Cancelled)
        {
            await OnEventCreated.InvokeAsync();
        }
    }
}