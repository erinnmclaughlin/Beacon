using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using BeaconUI.Core.Common.Http;
using ErrorOr;
using Microsoft.AspNetCore.Components;

namespace BeaconUI.Core.Projects.Pages;

public partial class ProjectDetailsPage
{
    [Inject]
    private IApiClient ApiClient { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter]
    public required string Code { get; set; }

    [Parameter]
    public string? Tab { get; set; }
    private ErrorOr<ProjectDto>? Project { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadProject();
    }

    private async Task LoadProject()
    {
        if (ProjectCode.FromString(Code) is not { } code)
        {
            NavigationManager.NavigateTo("l/projects");
            return;
        }

        Project = await ApiClient.SendAsync(new GetProjectByProjectCodeRequest { ProjectCode = code });
    }
}