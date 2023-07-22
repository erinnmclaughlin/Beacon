using MediatR;

namespace Beacon.Common.Requests;

public abstract class BeaconRequest<TRequest> : IRequest where TRequest : BeaconRequest<TRequest>
{
}

public abstract class BeaconRequest<TRequest, TResponse> : IRequest<TResponse> where TRequest : BeaconRequest<TRequest, TResponse>
{
}
