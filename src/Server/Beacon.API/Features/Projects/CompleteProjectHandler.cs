using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class CompleteProjectHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<CompleteProjectRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;

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