using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.GetCurrentUser;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Features.Auth;

public class GetCurrentUserRequestHandler : IApiRequestHandler<GetCurrentUserRequest, UserDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetCurrentUserRequestHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ErrorOr<UserDto>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        var result = _httpContextAccessor.HttpContext!.User.ToUserDto();
        return Task.FromResult(result is null ? Error.NotFound() : ErrorOrFactory.From(result));
    }
}
