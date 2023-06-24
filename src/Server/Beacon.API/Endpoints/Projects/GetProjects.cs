using Beacon.API.Persistence;
using Beacon.Common.Projects;
using Beacon.Common.Projects.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects;

public sealed class GetProjects : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet("projects", async ([AsParameters] GetProjectsRequest request, IMediator m, CancellationToken ct) =>
        {
            return Results.Ok(await m.Send(request, ct));
        });

        builder.WithTags(EndpointTags.Projects);
    }

    internal sealed class Handler : IRequestHandler<GetProjectsRequest, ProjectDto[]>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProjectDto[]> Handle(GetProjectsRequest request, CancellationToken ct)
        {
            var projects = await _dbContext.Projects
                .Where(x => x.LaboratoryId == request.LaboratoryId)
                .AsNoTracking()
                .ToArrayAsync(ct);

            return projects.Select(x => new ProjectDto
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                ProjectStatus = x.ProjectStatus,
                ProjectCode = x.ProjectCode.ToString()
            }).ToArray();

        }
    }
}
