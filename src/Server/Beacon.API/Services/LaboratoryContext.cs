using Beacon.App.Services;
using Beacon.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Beacon.API.Services;

public sealed class LaboratoryContext : ILabContext
{
    public Guid LaboratoryId { get; }

    public LaboratoryContext(IHttpContextAccessor httpContextAccessor)
    {
        var idValue = httpContextAccessor.HttpContext?.User.FindFirstValue(BeaconClaimTypes.LabId);
        LaboratoryId = Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
    }
}
