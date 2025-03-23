using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Notes;
using Beacon.Common.Services;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Notes;

internal sealed class AddProjectNoteRequestHandler(BeaconDbContext dbContext, ISessionContext sessionContext) : IBeaconRequestHandler<AddProjectNoteRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;
    private readonly ISessionContext _sessionContext = sessionContext;

    public async Task<ErrorOr<Success>> Handle(AddProjectNoteRequest request, CancellationToken ct)
    {
        if (!await _dbContext.Projects.AnyAsync(p => p.Id ==request.ProjectId, ct))
            return Error.NotFound("Project not found.");

        _dbContext.ProjectNotes.Add(new ProjectNote 
        {
             ProjectId = request.ProjectId, 
             Content = request.Content,
             CreatedById = _sessionContext.CurrentUser!.Id
        });
        
        await _dbContext.SaveChangesAsync(ct);

        return Result.Success;
    }
}
