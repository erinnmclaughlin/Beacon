using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.Contacts;
using Beacon.Common.Services;
using ErrorOr;

namespace Beacon.API.Endpoints.Projects.Contacts;

internal sealed class CreateProjectContactHandler : IBeaconRequestHandler<CreateProjectContactRequest>
{
    private readonly BeaconDbContext _dbContext;

    public CreateProjectContactHandler(BeaconDbContext dbContext, ILabContext labContext)
    {
        _dbContext = dbContext;
    }

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
