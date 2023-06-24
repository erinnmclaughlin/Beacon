using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.App.ValueObjects;
using Beacon.Common.Memberships;
using Beacon.Common.Projects.Requests;
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

    internal sealed class Handler : IRequestHandler<CreateProjectRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task Handle(CreateProjectRequest request, CancellationToken ct)
        {
            await EnsureUserIsAllowed(request.LaboratoryId, ct);

            _dbContext.Projects.Add(new Project
            {
                Id = Guid.NewGuid(),
                ProjectCode = await GenerateProjectCode(request, ct),
                CustomerName = request.CustomerName,
                CreatedById = _currentUser.UserId,
                LaboratoryId = request.LaboratoryId
            });

            await _dbContext.SaveChangesAsync(ct);
        }

        private async Task EnsureUserIsAllowed(Guid labId, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId;

            var membership = await _dbContext.Memberships
                .Where(m => m.MemberId == currentUserId && m.LaboratoryId == labId)
                .SingleOrDefaultAsync(ct);

            if (membership?.MembershipType is null or LaboratoryMembershipType.Member)
                throw new UserNotAllowedException();
        }

        private async Task<ProjectCode> GenerateProjectCode(CreateProjectRequest request, CancellationToken ct)
        {
            var lastSuffix = await _dbContext.Projects
                .Where(p => p.LaboratoryId == request.LaboratoryId && p.ProjectCode.CustomerCode == request.CustomerCode)
                .OrderBy(p => p.ProjectCode.Suffix)
                .Select(p => p.ProjectCode.Suffix)
                .LastOrDefaultAsync(ct);

            return new ProjectCode(request.CustomerCode, lastSuffix + 1);
        }
    }
}
