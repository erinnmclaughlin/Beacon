using Beacon.API.Persistence;
using Beacon.Common.Requests.Projects.Contacts;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.Contacts;

internal sealed class UpdateProjectContactHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<UpdateProjectContactRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Success>> Handle(UpdateProjectContactRequest request, CancellationToken ct)
    {
        var contact = await _dbContext.ProjectContacts
            .SingleAsync(x => x.Id == request.ContactId && x.ProjectId == request.ProjectId, ct);

        contact.Name = request.Name;
        contact.EmailAddress = string.IsNullOrWhiteSpace(request.EmailAddress) ? null : request.EmailAddress;
        contact.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber;

        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}
