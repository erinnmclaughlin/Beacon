using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;
using Beacon.Common.Requests.Projects.Events;
using BeaconUI.Core.Common;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects;

public partial class ProjectSchedule
{
    [Inject]
    private IApiClient ApiClient { get; set; } = default!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    private ErrorOr<ProjectEventDto[]>? ErrorOrEvents { get; set; }
    private ErrorOr<LaboratoryInstrumentDto[]>? ErrorOrInstruments { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadProjects();
        ErrorOrInstruments = await ApiClient.SendAsync(new GetLaboratoryInstrumentsRequest());
    }

    private static IEnumerable<TimelineItem<ProjectEventDto>> GetTimelineEvents(ProjectEventDto[] events)
    {
        return events.Select(e => new TimelineItem<ProjectEventDto> { Timestamp = e.ScheduledStart.DateTime, Value = e });
    }

    private async Task LoadProjects()
    {
        ErrorOrEvents = await ApiClient.SendAsync(new GetProjectEventsRequest { ProjectId = ProjectId });
    }

    private ErrorOr<LaboratoryInstrumentDto[]>? GetSuggestedInstruments(ProjectEventDto e)
    {
        if (ErrorOrInstruments is not { IsError: false } errorOrInstruments)
            return ErrorOrInstruments;

        var associatedInstrumentIds = e.AssociatedInstruments.Select(i => i.Id);
        return errorOrInstruments.Value.Where(i => !associatedInstrumentIds.Contains(i.Id)).ToArray();
    }

    private async Task Associate(ProjectEventDto e, LaboratoryInstrumentDto i)
    {
        var request = new AssociateInstrumentWithProjectEventRequest
        {
            ProjectEventId = e.Id,
            InstrumentId = i.Id
        };

        var response = await ApiClient.SendAsync(request);

        if (!response.IsError && ErrorOrEvents != null)
        {
            var newInstrumentList = ErrorOrEvents.Value.Value.Select(dto =>
            {
                if (dto.Id != e.Id)
                    return dto;

                return dto with { AssociatedInstruments = dto.AssociatedInstruments.Append(i).ToArray() };
            });

            ErrorOrEvents = newInstrumentList.ToArray();
        }
    }

    private async Task Unassociate(ProjectEventDto e, LaboratoryInstrumentDto i)
    {
        var request = new UnassociateInstrumentFromProjectEventRequest
        {
            ProjectEventId = e.Id,
            InstrumentId = i.Id
        };

        var response = await ApiClient.SendAsync(request);

        if (!response.IsError && ErrorOrEvents != null)
        {
            var newInstrumentList = ErrorOrEvents.Value.Value.Select(dto =>
            {
                if (dto.Id != e.Id)
                    return dto;

                return dto with { AssociatedInstruments = dto.AssociatedInstruments.Where(x => x.Id != i.Id).ToArray() };
            });

            ErrorOrEvents = newInstrumentList.ToArray();
        }
    }
}