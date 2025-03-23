using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Notes;
using Beacon.Common.Services;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Handlers.Projects.Notes;

internal sealed class GetProjectNotesHandler : IRequestHandler<GetProjectNotesRequest, ErrorOr<ProjectNoteDto[]>>
{
    private readonly BeaconDbContext _dbContext;
    private readonly ISessionContext _sessionContext;

    public GetProjectNotesHandler(BeaconDbContext dbContext, ISessionContext sessionContext)
    {
        _dbContext = dbContext;
        _sessionContext = sessionContext;
    }

    public async Task<ErrorOr<ProjectNoteDto[]>> Handle(GetProjectNotesRequest request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.LaboratoryId == _sessionContext.CurrentLab!.Id, cancellationToken);

        if (project is null)
            return Error.NotFound("Project not found");

        var notes = await _dbContext.Set<ProjectNote>()
            .Where(n => n.ProjectId == request.ProjectId)
            .OrderByDescending(n => n.CreatedOn)
            .Select(n => new ProjectNoteDto
            {
                Id = n.Id,
                Content = n.Content,
                CreatedOn = n.CreatedOn,
                CreatedBy = new LaboratoryMemberDto
                {
                    Id = n.CreatedBy.Id,
                    DisplayName = n.CreatedBy.DisplayName,
                    EmailAddress = n.CreatedBy.EmailAddress,
                    MembershipType = _sessionContext.CurrentLab!.MembershipType
                }
            })
            .ToArrayAsync(cancellationToken);

        return notes;
    }
} 