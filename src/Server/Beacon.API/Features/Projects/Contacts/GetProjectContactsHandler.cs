using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.Contacts;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Contacts;

internal sealed class GetProjectContactsHandler : IBeaconRequestHandler<GetProjectContactsRequest, ProjectContactDto[]>
{
    private readonly BeaconDbContext _dbContext;

    public GetProjectContactsHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<ProjectContactDto[]>> Handle(GetProjectContactsRequest request, CancellationToken ct)
    {
        return await _dbContext.ProjectContacts
            .Where(x => x.ProjectId == request.ProjectId)
            .Select(x => new ProjectContactDto
            {
                Id = x.Id,
                Name = x.Name,
                EmailAddress = x.EmailAddress,
                PhoneNumber = x.PhoneNumber
            })
            .ToArrayAsync(ct);
    }
}
