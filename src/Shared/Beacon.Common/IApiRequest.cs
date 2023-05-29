using ErrorOr;
using MediatR;

namespace Beacon.Common;

public interface IApiRequest<TResult> : IRequest<ErrorOr<TResult>>
{
}
