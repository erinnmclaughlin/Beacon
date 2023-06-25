using Beacon.Common;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Services;

public sealed class LaboratoryContext : ILabContext
{
    public Guid LaboratoryId { get; }

    public LaboratoryContext(IHttpContextAccessor httpContextAccessor)
    {
        var idValue = httpContextAccessor.HttpContext?.Request.Headers["X-LaboratoryId"];
        LaboratoryId = Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
    }
}
