using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects;

internal sealed class CompleteProjectHandler : IBeaconRequestHandler<CompleteProjectRequest>
{
    private readonly BeaconDbContext _dbContext;

    public CompleteProjectHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(CompleteProjectRequest request, CancellationToken ct)
    {
        var project = await _dbContext.Projects.SingleAsync(x => x.Id == request.ProjectId, ct);

        if (project.ProjectStatus is not ProjectStatus.Active)
            return Error.Validation(nameof(CompleteProjectRequest.ProjectId), "Inactive projects cannot be completed.");

        project.ProjectStatus = ProjectStatus.Completed;
        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}