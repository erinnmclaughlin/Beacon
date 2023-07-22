using Beacon.Common.Services;
using MediatR;

namespace Beacon.Common.Requests.Auth;

public sealed class GetSessionContextRequest : BeaconRequest<GetSessionContextRequest>, IRequest<SessionContext>
{
}
