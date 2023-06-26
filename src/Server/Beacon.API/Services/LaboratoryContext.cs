using Beacon.API.Persistence;
using Beacon.Common.Models;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Services;

public sealed class LaboratoryContext : ILabContext
{
    public Guid LaboratoryId { get; }
    public LaboratoryMembershipType? MembershipType { get; }

    public LaboratoryContext(ICurrentUser currentUser, BeaconDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        var idValue = httpContextAccessor.HttpContext?.Request.Headers["X-LaboratoryId"];
        LaboratoryId = Guid.TryParse(idValue, out var id) ? id : Guid.Empty;

        if (LaboratoryId != Guid.Empty)
        {
            MembershipType = dbContext.Memberships
                .SingleOrDefault(x => x.LaboratoryId == LaboratoryId && x.MemberId == currentUser.UserId)?
                .MembershipType;
        }
    }
}
