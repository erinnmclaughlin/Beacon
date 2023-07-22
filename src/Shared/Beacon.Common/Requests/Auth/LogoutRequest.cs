using MediatR;

namespace Beacon.Common.Requests.Auth;

public sealed class LogoutRequest : BeaconRequest<LogoutRequest>, IRequest { }
