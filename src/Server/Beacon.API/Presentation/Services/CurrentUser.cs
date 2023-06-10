using Beacon.API.App.Services;
using Beacon.API.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Beacon.API.Presentation.Services;

internal sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public CurrentUser(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public Guid UserId
    {
        get
        {
            var idValue = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(idValue, out var id) ? id : Guid.Empty;
        }
    }

    public async Task<User> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var userId = UserId;

        return await _unitOfWork
            .GetRepository<User>()
            .AsQueryable()
            .Where(u => u.Id == userId)
            .AsNoTracking()
            .FirstAsync(ct);
    }
}
