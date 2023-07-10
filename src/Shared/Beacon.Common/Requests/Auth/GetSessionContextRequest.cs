using Beacon.Common.Services;
using MediatR;

namespace Beacon.Common.Requests.Auth;

public sealed record GetSessionContextRequest : IRequest<SessionContext>;
