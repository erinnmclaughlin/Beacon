using Beacon.API.Auth.Services;
using Beacon.Common;
using Beacon.Common.Auth;
using Beacon.Common.Auth.Requests;
using ErrorOr;

namespace Beacon.API.Auth.RequestHandlers;

public class GetCurrentUserRequestHandler : IApiRequestHandler<GetCurrentUserRequest, UserDto>
{
    private readonly ICurrentUser _currentUser;

    public GetCurrentUserRequestHandler(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public Task<ErrorOr<UserDto>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(GetCurrentUser());
    }

    private ErrorOr<UserDto> GetCurrentUser()
    {
        var user = _currentUser.User.ToUserDto();
        return user is null ? Error.NotFound() : user;
    }
}
