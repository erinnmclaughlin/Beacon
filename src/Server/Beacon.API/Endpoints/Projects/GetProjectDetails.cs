using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Beacon.API.Endpoints.Projects;

public sealed class GetProjectDetails : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("projects/{idOrCode}", async (string idOrCode, IMediator m, CancellationToken ct) =>
        {
            if (Guid.TryParse(idOrCode, out var id))
            {
                var request = new GetProjectByIdRequest { ProjectId = id };
                var projectOrNull = await m.Send(request, ct);
                return projectOrNull is null ? Results.NotFound() : Results.Ok(projectOrNull);
            }

            if (ProjectCode.FromString(idOrCode) is { } code)
            {
                var request = new GetProjectByProjectCodeRequest { ProjectCode = code };
                var projectOrNull = await m.Send(request, ct);
                return projectOrNull is null ? Results.NotFound() : Results.Ok(projectOrNull);
            }

            return Results.BadRequest();
        });
    }

    internal sealed class Handler : IRequestHandler<GetProjectByIdRequest, ProjectDto?>, IRequestHandler<GetProjectByProjectCodeRequest, ProjectDto?>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdRequest request, CancellationToken ct)
        {
            return await GetAsync(x => x.Id == request.ProjectId, ct);
        }

        public async Task<ProjectDto?> Handle(GetProjectByProjectCodeRequest request, CancellationToken ct)
        {
            return await GetAsync(x =>
                x.ProjectCode.CustomerCode == request.ProjectCode.CustomerCode && 
                x.ProjectCode.Suffix == request.ProjectCode.Suffix, ct);
        }

        private async Task<ProjectDto?> GetAsync(Expression<Func<Project, bool>> filter, CancellationToken ct)
        {
            var project = await _dbContext.Projects.Include(x => x.LeadAnalyst).AsNoTracking().SingleOrDefaultAsync(filter, ct);

            return project is null ? null : new ProjectDto
            {
                Id = project.Id,
                CustomerName = project.CustomerName,
                ProjectStatus = project.ProjectStatus,
                ProjectCode = project.ProjectCode.ToString(),
                LeadAnalyst = project.LeadAnalyst == null ? null : new ProjectDto.LeadAnalystDto
                {
                    Id = project.LeadAnalyst.Id,
                    DisplayName = project.LeadAnalyst.DisplayName
                }
            };
        }
    }
}
