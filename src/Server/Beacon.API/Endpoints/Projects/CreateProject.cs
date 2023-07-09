using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Services;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects;

public sealed class CreateProject : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<CreateProjectRequest>("projects").WithTags(EndpointTags.Projects);
    }

    public sealed class Validator : AbstractValidator<CreateProjectRequest>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Validator(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;

            RuleFor(x => x.LeadAnalystId)
                .MustAsync(BeAuthorized)
                .WithMessage("Lead analyst must have at least an analyst role.");
        }

        private async Task<bool> BeAuthorized(Guid? analystId, CancellationToken ct)
        {
            if (analystId == null)
                return true;

            var membership = await _dbContext.Memberships
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.LaboratoryId == _labContext.LaboratoryId && m.MemberId == analystId.Value, ct);

            return membership?.MembershipType is >= LaboratoryMembershipType.Analyst;
        }
    }

    internal sealed class Handler : IRequestHandler<CreateProjectRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext, ILabContext labContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task Handle(CreateProjectRequest request, CancellationToken ct)
        {
            _dbContext.Projects.Add(new Project
            {
                Id = Guid.NewGuid(),
                ProjectCode = await GenerateProjectCode(request, ct),
                CustomerName = request.CustomerName,
                CreatedById = _currentUser.UserId,
                LaboratoryId = _labContext.LaboratoryId,
                LeadAnalystId = request.LeadAnalystId
            });

            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task<ProjectCode> GenerateProjectCode(CreateProjectRequest request, CancellationToken ct)
        {
            var lastSuffix = await _dbContext.Projects
                .Where(p => p.LaboratoryId == _labContext.LaboratoryId && p.ProjectCode.CustomerCode == request.CustomerCode)
                .OrderBy(p => p.ProjectCode.Suffix)
                .Select(p => p.ProjectCode.Suffix)
                .LastOrDefaultAsync(ct);

            return new ProjectCode(request.CustomerCode, lastSuffix + 1);
        }
    }
}
