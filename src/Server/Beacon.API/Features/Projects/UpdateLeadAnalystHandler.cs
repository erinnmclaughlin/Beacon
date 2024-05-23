using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class UpdateLeadAnalystHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<UpdateLeadAnalystRequest>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<Success>> Handle(UpdateLeadAnalystRequest request, CancellationToken ct)
    {
        if (request.AnalystId is { } analystId && !await IsValidLeadAnalyst(analystId, ct))
            return Error.Validation(
                nameof(UpdateLeadAnalystRequest.AnalystId), 
                "Lead analyst must have at least an analyst role.");

        var project = await _dbContext.Projects.SingleAsync(x => x.Id == request.ProjectId, ct);

        project.LeadAnalystId = request.AnalystId;
        await _dbContext.SaveChangesAsync(ct);
        return Result.Success;
    }

    private async Task<bool> IsValidLeadAnalyst(Guid analystId, CancellationToken ct)
    {
        var membership = await _dbContext.Memberships
            .AsNoTracking()
            .SingleOrDefaultAsync(m => m.MemberId == analystId, ct);

        return membership?.MembershipType is >= LaboratoryMembershipType.Analyst;
    }
}
