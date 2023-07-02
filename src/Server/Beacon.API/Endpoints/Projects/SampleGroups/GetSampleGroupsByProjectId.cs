using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.SampleGroups;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Endpoints.Projects.SampleGroups;

public sealed class GetSampleGroupsByProjectId : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapGet("projects/{projectId:guid}/sample-groups", async (Guid projectId, IMediator m, CancellationToken ct) =>
        {
            var sampleGroups = await m.Send(new GetSampleGroupsByProjectIdRequest { ProjectId = projectId }, ct);
            return Results.Ok(sampleGroups);
        });

        builder.WithTags(EndpointTags.SampleGroups);
    }

    internal sealed class Handler : IRequestHandler<GetSampleGroupsByProjectIdRequest, SampleGroupDto[]>
    {
        private readonly BeaconDbContext _dbContext;

        public Handler(BeaconDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SampleGroupDto[]> Handle(GetSampleGroupsByProjectIdRequest request, CancellationToken ct)
        {
            return await _dbContext.SampleGroups
                .Where(x => x.ProjectId == request.ProjectId)
                .Select(x => new SampleGroupDto
                {
                    Id = x.Id,
                    ContainerType = x.ContainerType,
                    IsHazardous = x.IsHazardous,
                    IsLightSensitive = x.IsLightSensitive,
                    Notes = x.Notes,
                    Quantity = x.Quantity,
                    SampleName = x.SampleName,
                    TargetStorageHumidity = x.TargetStorageHumidity,
                    TargetStorageTemperature = x.TargetStorageTemperature,
                    Volume = x.Volume
                })
                .ToArrayAsync(ct);
        }
    }
}
