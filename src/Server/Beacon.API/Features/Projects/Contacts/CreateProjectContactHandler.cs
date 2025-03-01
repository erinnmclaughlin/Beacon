using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Contacts;
using ErrorOr;

namespace Beacon.API.Features.Projects.Contacts;

internal sealed class CreateProjectContactHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<CreateProjectContactRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Success>> Handle(CreateProjectContactRequest request, CancellationToken ct)
    {
        _dbContext.ProjectContacts.Add(new ProjectContact
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            EmailAddress = string.IsNullOrWhiteSpace(request.EmailAddress) ? null : request.EmailAddress,
            PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber,
            ProjectId = request.ProjectId
        });

        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}
