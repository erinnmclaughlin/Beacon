using Beacon.Common.Models;
using MediatR;

namespace Beacon.Common.Requests.Auth;

public sealed record GetCurrentUserRequest : IRequest<CurrentUserDto>;
