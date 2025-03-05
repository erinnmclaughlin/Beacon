using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Requests.Instruments;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Requests.Projects.Events;
using BeaconUI.Core.Common;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects;

public partial class ProjectSchedule
{
    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    [Parameter, EditorRequired]
    public required Guid ProjectId { get; set; }

    private ErrorOr<PagedList<LaboratoryEventDto>>? ErrorOrEvents { get; set; }
    private ErrorOr<LaboratoryInstrumentDto[]>? ErrorOrInstruments { get; set; }

    private bool ShowPastEvents { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadProjects();
        ErrorOrInstruments = await ApiClient.SendAsync(new GetLaboratoryInstrumentsRequest());
    }

    private int CompletedEventCount => ErrorOrEvents?.Value.Items.Count(x => x.IsCompletedOnOrBefore(DateTime.Now)) ?? 0;

    private IEnumerable<TimelineItem<LaboratoryEventDto>> GetTimelineEvents(IEnumerable<LaboratoryEventDto> events)
    {
        return events
            .Where(e => ShowPastEvents || !e.IsCompletedOnOrBefore(DateTime.Now))
            .Select(e => new TimelineItem<LaboratoryEventDto> { Timestamp = e.ScheduledStart.DateTime, Value = e });
    }

    private async Task LoadProjects()
    {
        ErrorOrEvents = await ApiClient.SendAsync(new GetLaboratoryEventsRequest { ProjectIds = [ProjectId] });
    }

    private ErrorOr<LaboratoryInstrumentDto[]>? GetSuggestedInstruments(LaboratoryEventDto e)
    {
        if (ErrorOrInstruments is not { IsError: false } errorOrInstruments)
            return ErrorOrInstruments;

        var associatedInstrumentIds = e.AssociatedInstruments.Select(i => i.Id);
        return errorOrInstruments.Value.Where(i => !associatedInstrumentIds.Contains(i.Id)).ToArray();
    }

    private async Task Associate(LaboratoryEventDto e, LaboratoryInstrumentDto i)
    {
        var request = new LinkInstrumentToProjectEventRequest
        {
            ProjectEventId = e.Id,
            InstrumentId = i.Id
        };

        var response = await ApiClient.SendAsync(request);

        if (!response.IsError && ErrorOrEvents != null)
        {
            var current = ErrorOrEvents.Value.Value;

            var newInstrumentList = current.Items.Select(dto =>
            {
                if (dto.Id != e.Id)
                    return dto;

                return dto with { AssociatedInstruments = dto.AssociatedInstruments.Append(i).ToArray() };
            });

            ErrorOrEvents = new PagedList<LaboratoryEventDto>(newInstrumentList.ToArray(), current.TotalPages, current.CurrentPage, current.PageSize);
        }
    }

    private async Task Unassociate(LaboratoryEventDto e, LaboratoryInstrumentDto i)
    {
        var request = new UnlinkInstrumentFromProjectEventRequest
        {
            ProjectEventId = e.Id,
            InstrumentId = i.Id
        };

        var response = await ApiClient.SendAsync(request);

        if (!response.IsError && ErrorOrEvents != null)
        {
            var current = ErrorOrEvents.Value.Value;

            var newInstrumentList = current.Items.Select(dto =>
            {
                if (dto.Id != e.Id)
                    return dto;

                return dto with { AssociatedInstruments = dto.AssociatedInstruments.Where(x => x.Id != i.Id).ToArray() };
            });

            ErrorOrEvents = new PagedList<LaboratoryEventDto>(newInstrumentList.ToArray(), current.TotalPages, current.CurrentPage, current.PageSize);
        }
    }
}