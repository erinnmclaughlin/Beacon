using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using ErrorOr;

namespace Beacon.API.Features.Laboratories;

internal sealed class CreateLaboratoryHandler : IBeaconRequestHandler<CreateLaboratoryRequest>
{
    private readonly ISessionContext _context;
    private readonly BeaconDbContext _dbContext;

    public CreateLaboratoryHandler(ISessionContext currentUser, BeaconDbContext dbContext)
    {
        _context = currentUser;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(CreateLaboratoryRequest request, CancellationToken ct)
    {
        var lab = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = request.LaboratoryName
        };

        lab.AddMember(_context.UserId, LaboratoryMembershipType.Admin);

        _dbContext.Laboratories.Add(lab);
        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }
}
