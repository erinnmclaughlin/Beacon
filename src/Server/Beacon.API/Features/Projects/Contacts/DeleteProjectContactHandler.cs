using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects.Contacts;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Contacts;

internal sealed class DeleteProjectContactHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<DeleteProjectContactRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Success>> Handle(DeleteProjectContactRequest request, CancellationToken ct)
    {
        await _dbContext.ProjectContacts
            .Where(x => x.Id == request.ContactId && x.ProjectId == request.ProjectId)
            .ExecuteDeleteAsync(ct);

        return Result.Success;
    }
}
