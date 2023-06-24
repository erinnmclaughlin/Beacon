using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.Exceptions;
using Beacon.App.Services;
using Beacon.Common.Memberships;
using Beacon.Common.Projects;
using Beacon.Common.Projects.Requests;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects;

public sealed class CompleteProject : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost<CompleteProjectRequest>("projects/complete").WithTags(EndpointTags.Projects);
    }

    internal sealed class Handler : IRequestHandler<CompleteProjectRequest>
    {
        private readonly ICurrentUser _currentUser;
        private readonly BeaconDbContext _dbContext;

        public Handler(ICurrentUser currentUser, BeaconDbContext dbContext)
        {
            _currentUser = currentUser;
            _dbContext = dbContext;
        }

        public async Task Handle(CompleteProjectRequest request, CancellationToken ct)
        {
            var project = await _dbContext.Projects.SingleAsync(x => x.Id == request.ProjectId, ct);

            await EnsureUserIsAllowed(project.LaboratoryId, ct);

            if (project.ProjectStatus is ProjectStatus.Completed)
                return;

            if (project.ProjectStatus is ProjectStatus.Canceled)
                throw new BeaconValidationException(nameof(Project.ProjectStatus), "Projects that have been canceled cannot be marked as complete.");

            project.ProjectStatus = ProjectStatus.Completed;
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
    }
}
