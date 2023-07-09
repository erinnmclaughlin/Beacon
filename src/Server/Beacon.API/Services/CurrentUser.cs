using Beacon.Common;
using Beacon.Common.Models;
using Beacon.Common.Services;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Services;

internal sealed class CurrentUser : ICurrentUser
{
    public Guid UserId { get; }
    public LaboratoryMembershipType? MembershipType { get; }

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;

        UserId = user?.GetUserId() ?? Guid.Empty;
        MembershipType = user?.GetMembershipType();
    }
}
