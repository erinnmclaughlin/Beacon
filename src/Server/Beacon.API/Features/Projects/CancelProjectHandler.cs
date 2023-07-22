using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class CancelProjectHandler : IBeaconRequestHandler<CancelProjectRequest>
{
    private readonly BeaconDbContext _dbContext;

    public CancelProjectHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(CancelProjectRequest request, CancellationToken ct)
    {
        var project = await _dbContext.Projects.SingleAsync(x => x.Id == request.ProjectId, ct);

        if (project.ProjectStatus is not ProjectStatus.Active)
            return Error.Validation(nameof(CancelProjectRequest.ProjectId), "Inactive projects cannot be canceled.");

        project.ProjectStatus = ProjectStatus.Canceled;
        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}
