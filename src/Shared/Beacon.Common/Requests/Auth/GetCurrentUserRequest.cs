using Beacon.Common.Services;
using MediatR;

namespace Beacon.Common.Requests.Auth;

public sealed record GetCurrentUserRequest : IRequest<SessionContext>;
