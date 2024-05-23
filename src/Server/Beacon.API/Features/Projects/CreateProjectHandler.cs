using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Services;
using ErrorOr;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects;

internal sealed class CreateProjectHandler(BeaconDbContext dbContext, ISessionContext sessionContext) : IBeaconRequestHandler<CreateProjectRequest, ProjectCode>
{
    private readonly BeaconDbContext _dbContext = dbContext;
    private readonly ISessionContext _sessionContext = sessionContext;

    public async Task<ErrorOr<ProjectCode>> Handle(CreateProjectRequest request, CancellationToken ct)
    {
        if (request.LeadAnalystId is { } leadId && !await IsValidLeadAnalyst(leadId, ct))
            return Error.Validation(nameof(CreateProjectRequest.LeadAnalystId), "Lead analyst must have at least an analyst role.");

        var project = new Project
        {
            Id = Guid.NewGuid(),
            ProjectCode = await GenerateProjectCode(request, ct),
            CustomerName = request.CustomerName,
            CreatedById = _sessionContext.CurrentUser.Id,
            LeadAnalystId = request.LeadAnalystId
        };

        if (!string.IsNullOrWhiteSpace(request.Application))
        {
            var appId = await _dbContext.ProjectApplications.Where(x => x.Name == request.Application).Select(x => x.Id).SingleOrDefaultAsync(ct);

            if (appId == Guid.Empty)
            {
                project.TaggedApplications.Add(new ProjectApplicationTag
                {
                    Application = new() { Name = request.Application }
                });
            }
            else
            {
                project.TaggedApplications.Add(new ProjectApplicationTag
                {
                    ApplicationId = appId
                });
            }
        }

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync(ct);
        return project.ProjectCode;
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
        var today = DateTime.Today.ToString("yyyyMM");

        var lastSuffix = await _dbContext.Projects
            .Where(p => p.ProjectCode.CustomerCode == request.CustomerCode && p.ProjectCode.Date == today)
            .OrderBy(p => p.ProjectCode.Suffix)
            .Select(p => p.ProjectCode.Suffix)
            .LastOrDefaultAsync(ct);

        return new ProjectCode(request.CustomerCode, today, lastSuffix + 1);
    }
}