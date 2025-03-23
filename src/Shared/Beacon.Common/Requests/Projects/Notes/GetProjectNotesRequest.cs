using Beacon.Common.Models;
using ErrorOr;
using MediatR;

namespace Beacon.Common.Requests.Projects.Notes;

public sealed class GetProjectNotesRequest : BeaconRequest<GetProjectNotesRequest, ProjectNoteDto[]>
{
    public GetProjectNotesRequest()
    {
    }

    public GetProjectNotesRequest(Guid projectId)
    {
        ProjectId = projectId;
    }

    public Guid ProjectId { get; init; }
} 