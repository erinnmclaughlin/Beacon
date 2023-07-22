using Beacon.Common.Requests;
using ErrorOr;
using MediatR;

namespace Beacon.API.Features;

public interface IBeaconRequestHandler { }

public interface IBeaconRequestHandler<TRequest> :
    IBeaconRequestHandler,
    IRequestHandler<TRequest, ErrorOr<Success>>
    where TRequest : BeaconRequest<TRequest>, new() { }

public interface IBeaconRequestHandler<TRequest, TResponse> : 
    IBeaconRequestHandler, 
    IRequestHandler<TRequest, ErrorOr<TResponse>>
    where TRequest : BeaconRequest<TRequest, TResponse>, new() { }

