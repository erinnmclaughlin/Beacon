using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.App.ValueObjects;
using Beacon.Common.Projects;
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
                var projectOrNull = await m.Send(new ById(id), ct);
                return projectOrNull is null ? Results.NotFound() : Results.Ok(projectOrNull);
            }

            if (ProjectCode.FromString(idOrCode) is { } code)
            {
                var projectOrNull = await m.Send(new ByCode(code), ct);
                return projectOrNull is null ? Results.NotFound() : Results.Ok(projectOrNull);
            }

            return Results.BadRequest();
        });
    }

    public sealed record ById(Guid Id) : IRequest<ProjectDto?>;
    public sealed record ByCode(ProjectCode ProjectCode) : IRequest<ProjectDto?>;

    internal sealed class Handler : IRequestHandler<ById, ProjectDto?>, IRequestHandler<ByCode, ProjectDto?>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProjectDto?> Handle(ById request, CancellationToken ct)
        {
            return await GetAsync(x => x.Id == request.Id, ct);
        }

        public async Task<ProjectDto?> Handle(ByCode request, CancellationToken ct)
        {
            return await GetAsync(x =>
                x.ProjectCode.CustomerCode == request.ProjectCode.CustomerCode && 
                x.ProjectCode.Suffix == request.ProjectCode.Suffix, ct);
        }

        private async Task<ProjectDto?> GetAsync(Expression<Func<Project, bool>> filter, CancellationToken ct)
        {
            var project = await _dbContext.Projects
                .AsNoTracking()
                .SingleOrDefaultAsync(filter, ct);

            return project is null ? null : new ProjectDto
            {
                Id = project.Id,
                CustomerName = project.CustomerName,
                ProjectStatus = project.ProjectStatus,
                ProjectCode = project.ProjectCode.ToString()
            };
        }
    }
}
