using ErrorOr;
using MediatR;

namespace Beacon.Common;

public interface IApiRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, ErrorOr<TResponse>>
    where TRequest : IApiRequest<TResponse>
{
}
