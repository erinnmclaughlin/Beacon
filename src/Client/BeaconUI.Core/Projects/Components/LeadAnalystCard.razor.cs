using Beacon.Common.Models;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Components;

public partial class LeadAnalystCard
{
    [Parameter]
    public ProjectDto.LeadAnalystDto? LeadAnalyst { get; set; }

    [Parameter]
    public EventCallback<ProjectDto.LeadAnalystDto?> LeadAnalystChanged { get; set; }

    private bool IsBusy { get; set; }

    private async Task UpdateLeadAnalyst(LaboratoryMemberDto? member)
    {
        if (IsBusy) return;
        IsBusy = true;

        if (member is null && LeadAnalyst is null)
            return;
        
        await LeadAnalystChanged.InvokeAsync(member is null || member.Id == LeadAnalyst?.Id ? null : new() { Id = member.Id, DisplayName = member.DisplayName });
        IsBusy = false;
    }
}