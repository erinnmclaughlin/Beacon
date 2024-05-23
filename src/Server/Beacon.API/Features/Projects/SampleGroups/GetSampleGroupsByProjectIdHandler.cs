using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Requests.Projects.SampleGroups;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Beacon.API.Features.Projects.SampleGroups;

internal sealed class GetSampleGroupsByProjectIdHandler(BeaconDbContext dbContext) : IBeaconRequestHandler<GetSampleGroupsByProjectIdRequest, SampleGroupDto[]>
{
    private readonly BeaconDbContext _dbContext = dbContext;

    public async Task<ErrorOr<SampleGroupDto[]>> Handle(GetSampleGroupsByProjectIdRequest request, CancellationToken ct)
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
