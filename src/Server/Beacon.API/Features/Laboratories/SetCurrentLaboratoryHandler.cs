using Beacon.API.Persistence;
using Beacon.API.Services;
using Beacon.Common.Requests.Laboratories;
using Beacon.Common.Services;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Laboratories;

internal sealed class SetCurrentLaboratoryHandler(ISessionContext context, BeaconDbContext dbContext, ISignInManager signInManager) : IBeaconRequestHandler<SetCurrentLaboratoryRequest>
{
    private readonly ISessionContext _context = context;
    private readonly BeaconDbContext _dbContext = dbContext;
    private readonly ISignInManager _signInManager = signInManager;

    public async Task<ErrorOr<Success>> Handle(SetCurrentLaboratoryRequest request, CancellationToken ct)
    {
        var newContext = await _dbContext.Memberships
            .Where(m => m.LaboratoryId == request.LaboratoryId && m.MemberId == _context.UserId)
            .Select(m => new SessionContext
            {
                CurrentUser = new() { Id = m.Member.Id, DisplayName = m.Member.DisplayName },
                CurrentLab = new() { Id = m.Laboratory.Id, Name = m.Laboratory.Name, MembershipType = m.MembershipType }
            })
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(ct);

        if (newContext is null)
            return BeaconError.Forbid("The current user is not a member of the specified lab.");

        await _signInManager.SignInAsync(newContext.ToClaimsPrincipal());
        return Result.Success;
    }
}
