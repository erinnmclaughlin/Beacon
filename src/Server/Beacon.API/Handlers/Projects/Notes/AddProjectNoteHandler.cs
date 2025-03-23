using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Notes;
using Beacon.Common.Services;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Handlers.Projects.Notes;

internal sealed class AddProjectNoteHandler : IRequestHandler<AddProjectNoteRequest, ErrorOr<ProjectNoteDto>>
{
    private readonly BeaconDbContext _dbContext;
    private readonly ISessionContext _sessionContext;

    public AddProjectNoteHandler(BeaconDbContext dbContext, ISessionContext sessionContext)
    {
        _dbContext = dbContext;
        _sessionContext = sessionContext;
    }

    public async Task<ErrorOr<ProjectNoteDto>> Handle(AddProjectNoteRequest request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId && p.LaboratoryId == _sessionContext.CurrentLab!.Id, cancellationToken);

        if (project is null)
            return Error.NotFound("Project not found");

        var note = new ProjectNote
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            ProjectId = project.Id,
            LaboratoryId = project.LaboratoryId,
            CreatedById = _sessionContext.UserId,
            CreatedOn = DateTime.UtcNow
        };

        _dbContext.Add(note);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var createdBy = await _dbContext.Users
            .Where(u => u.Id == note.CreatedById)
            .Select(u => new LaboratoryMemberDto
            {
                Id = u.Id,
                DisplayName = u.DisplayName,
                EmailAddress = u.EmailAddress,
                MembershipType = _sessionContext.CurrentLab!.MembershipType
            })
            .FirstAsync(cancellationToken);

        return new ProjectNoteDto
        {
            Id = note.Id,
            Content = note.Content,
            CreatedOn = note.CreatedOn,
            CreatedBy = createdBy
        };
    }
} 