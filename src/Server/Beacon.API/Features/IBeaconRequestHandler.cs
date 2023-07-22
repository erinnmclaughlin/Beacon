using Beacon.Common.Requests;
using ErrorOr;
using MediatR;

namespace Beacon.API.Features;

public interface IBeaconRequestHandler { }

public interface IBeaconRequestHandler<TRequest, TResponse> : 
    IBeaconRequestHandler, 
    IRequestHandler<TRequest, ErrorOr<TResponse>>
    where TRequest : BeaconRequest<TRequest, TResponse> { }

public interface IBeaconRequestHandler<TRequest> :
    IBeaconRequestHandler<TRequest, Success> 
    where TRequest : BeaconRequest<TRequest> { }
