using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using Beacon.Common.Services;
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
        private readonly ILabContext _labContext;

        public Handler(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task<ProjectDto[]> Handle(GetProjectsRequest request, CancellationToken ct)
        {
            var projects = await _dbContext.Projects
                .Include(x => x.LeadAnalyst)
                .Where(x => x.LaboratoryId == _labContext.CurrentLab.Id)
                .AsNoTracking()
                .ToArrayAsync(ct);

            return projects.Select(x => new ProjectDto
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                ProjectStatus = x.ProjectStatus,
                ProjectCode = x.ProjectCode.ToString(),
                LeadAnalyst = x.LeadAnalyst == null ? null : new ProjectDto.LeadAnalystDto
                {
                    Id = x.LeadAnalyst.Id,
                    DisplayName = x.LeadAnalyst.DisplayName
                }
            }).ToArray();
        }
    }
}
