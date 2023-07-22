using Beacon.API.Persistence;
using Beacon.API.Persistence.Entities;
using Beacon.Common.Requests.Projects.SampleGroups;
using ErrorOr;

namespace Beacon.API.Endpoints.Projects.SampleGroups;

internal sealed class CreateSampleGroupHandler : IBeaconRequestHandler<CreateSampleGroupRequest>
{
    private readonly BeaconDbContext _dbContext;

    public CreateSampleGroupHandler(BeaconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(CreateSampleGroupRequest request, CancellationToken ct)
    {
        _dbContext.SampleGroups.Add(new SampleGroup
        {
            Id = Guid.NewGuid(),
            ProjectId = request.ProjectId,
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
        return Result.Success;
    }
}
