using Beacon.API.Auth.Services;
using Beacon.API.Entities;
using Beacon.API.Persistence;
using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Requests;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Laboratories.RequestHandlers;

internal sealed class CreateLaboratoryRequestHandler : IApiRequestHandler<CreateLaboratoryRequest, LaboratoryDto>
{
    private readonly ICurrentUser _currentUser;
    private readonly BeaconDbContext _dbContext;

    public CreateLaboratoryRequestHandler(ICurrentUser currentUser, BeaconDbContext dbContext)
    {
        _currentUser = currentUser;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<LaboratoryDto>> Handle(CreateLaboratoryRequest request, CancellationToken ct)
    {
        var currentUser = await _dbContext.Users.FirstAsync(u => u.Id == _currentUser.UserId, ct);

        var lab = new Laboratory
        {
            Id = Guid.NewGuid(),
            Name = request.Name
        };

        lab.AddMember(currentUser, LaboratoryMembershipType.Admin);

        _dbContext.Laboratories.Add(lab);
        await _dbContext.SaveChangesAsync(ct);

        return new LaboratoryDto
        {
            Id = lab.Id,
            Name = lab.Name
        };
    }
}
