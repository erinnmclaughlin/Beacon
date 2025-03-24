using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Notes;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Notes;

internal sealed class GetProjectNotesRequestHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<GetProjectNotesRequest, ProjectNoteDto[]>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<ProjectNoteDto[]>> Handle(GetProjectNotesRequest request, CancellationToken ct)
    {
        if (!await _dbContext.Projects.AnyAsync(p => p.Id == request.ProjectId, ct))
            return Error.NotFound("Project not found.");

        return await _dbContext.ProjectNotes
            .Where(n => n.ProjectId == request.ProjectId)
            .Select(n => new ProjectNoteDto
            {
                Id = n.Id,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                CreatedById = n.CreatedById,
                CreatedByDisplayName = n.CreatedBy.DisplayName
            })
            .ToArrayAsync(ct);
    }
}

