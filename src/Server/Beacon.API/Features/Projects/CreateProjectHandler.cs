using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Services;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class CreateProjectHandler : IBeaconRequestHandler<CreateProjectRequest>
{
    private readonly BeaconDbContext _dbContext;
    private readonly ISessionContext _sessionContext;

    public CreateProjectHandler(BeaconDbContext dbContext, ISessionContext sessionContext)
    {
        _dbContext = dbContext;
        _sessionContext = sessionContext;
    }

    public async Task<ErrorOr<Success>> Handle(CreateProjectRequest request, CancellationToken ct)
    {
        if (request.LeadAnalystId is { } leadId && !await IsValidLeadAnalyst(leadId, ct))
            return Error.Validation(nameof(CreateProjectRequest.LeadAnalystId), "Lead analyst must have at least an analyst role.");

        _dbContext.Projects.Add(new Project
        {
            Id = Guid.NewGuid(),
            ProjectCode = await GenerateProjectCode(request, ct),
            CustomerName = request.CustomerName,
            CreatedById = _sessionContext.CurrentUser.Id,
            LeadAnalystId = request.LeadAnalystId
        });

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

    private async Task<ProjectCode> GenerateProjectCode(CreateProjectRequest request, CancellationToken ct)
    {
        var lastSuffix = await _dbContext.Projects
            .Where(p => p.ProjectCode.CustomerCode == request.CustomerCode)
            .OrderBy(p => p.ProjectCode.Suffix)
            .Select(p => p.ProjectCode.Suffix)
            .LastOrDefaultAsync(ct);

        return new ProjectCode(request.CustomerCode, lastSuffix + 1);
    }
}