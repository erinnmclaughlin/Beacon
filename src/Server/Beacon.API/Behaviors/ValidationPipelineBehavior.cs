using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Beacon.API.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var validationErrors = await GetValidationErrorsAsync(request, cancellationToken);

        if (validationErrors.Any())
            throw new ValidationException(validationErrors);

        return await next();
    }

    private async Task<List<ValidationFailure>> GetValidationErrorsAsync(TRequest request, CancellationToken ct)
    {
        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, ct);

            if (result.Errors.Any())
                failures.AddRange(result.Errors);
        }

        return failures;
    }
}
