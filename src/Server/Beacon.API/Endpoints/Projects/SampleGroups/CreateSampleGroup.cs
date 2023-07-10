using Beacon.API.Persistence;
using Beacon.App.Entities;
using Beacon.Common.Requests.Projects.SampleGroups;
using Beacon.Common.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Beacon.API.Endpoints.Projects.SampleGroups;

public sealed class CreateSampleGroup : IBeaconEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var builder = app.MapPost("projects/{projectId:guid}/sample-groups", async (Guid projectId, CreateSampleGroupRequest request, IMediator m, CancellationToken ct) =>
        {
            if (request.ProjectId != projectId)
                return Results.BadRequest();

            await m.Send(request, ct);
            return Results.NoContent();
        });

        builder.WithTags(EndpointTags.SampleGroups);
    }

    internal sealed class Handler : IRequestHandler<CreateSampleGroupRequest>
    {
        private readonly BeaconDbContext _dbContext;
        private readonly ILabContext _labContext;

        public Handler(BeaconDbContext dbContext, ILabContext labContext)
        {
            _dbContext = dbContext;
            _labContext = labContext;
        }

        public async Task Handle(CreateSampleGroupRequest request, CancellationToken ct)
        {
            _dbContext.SampleGroups.Add(new SampleGroup
            {
                Id = Guid.NewGuid(),
                ProjectId = request.ProjectId,
                LaboratoryId = _labContext.CurrentLab.Id,
                SampleName = request.SampleName,
                ContainerType = request.ContainerType,
                IsHazardous = request.IsHazardous,
                IsLightSensitive = request.IsLightSensitive,
                Notes = request.Notes,
                Quantity = request.Quantity,
                TargetStorageHumidity = request.TargetStorageHumidity,
                TargetStorageTemperature = request.TargetStorageTemperature,
                Volume = request.Volume
            });

            await _dbContext.SaveChangesAsync(ct);
        }
    }
}
