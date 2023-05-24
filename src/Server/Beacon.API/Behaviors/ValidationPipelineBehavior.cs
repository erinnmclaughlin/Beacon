using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Beacon.API.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (_validators.Any())
            await Validate(request, ct);

        return await next();
    }

    private async Task Validate(TRequest request, CancellationToken ct)
    {
        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, ct);

            if (result.Errors.Any())
                failures.AddRange(result.Errors);
        }

        if (failures.Any())
            throw new ValidationException(failures);
    }
}
