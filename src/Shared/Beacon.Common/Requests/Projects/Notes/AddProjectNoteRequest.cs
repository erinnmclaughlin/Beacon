using Beacon.Common.Models;

namespace Beacon.Common.Requests.Projects.Notes;

public sealed class AddProjectNoteRequest : BeaconRequest<AddProjectNoteRequest, ProjectNoteDto>
{
    public AddProjectNoteRequest()
    {
    }

    public AddProjectNoteRequest(Guid projectId, string content)
    {
        ProjectId = projectId;
        Content = content;
    }

    public Guid ProjectId { get; set; }
    public string Content { get; set; } = string.Empty;
} 