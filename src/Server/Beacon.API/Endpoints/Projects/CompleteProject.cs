using Beacon.API.Persistence;
using Beacon.Common.Projects;
using Beacon.Common.Requests.Projects;
using FluentValidation;
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

    public sealed class Validator : AbstractValidator<CompleteProjectRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public Validator(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.ProjectId)
                .MustAsync(BeActive).WithMessage("Inactive projects cannot be completed.");
        }

        private async Task<bool> BeActive(Guid projectId, CancellationToken ct)
        {
            var project = await _dbContext.Projects
                .AsNoTracking()
                .SingleAsync(x => x.Id == projectId, ct);

            return project.ProjectStatus is ProjectStatus.Active;
        }
    }

    internal sealed class Handler : IRequestHandler<CompleteProjectRequest>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(CompleteProjectRequest request, CancellationToken ct)
        {
            var project = await _dbContext.Projects.SingleAsync(x => x.Id == request.ProjectId, ct);

            if (project.ProjectStatus is not ProjectStatus.Completed)
            {
                project.ProjectStatus = ProjectStatus.Completed;
                await _dbContext.SaveChangesAsync(ct);
            }
        }
    }
}
