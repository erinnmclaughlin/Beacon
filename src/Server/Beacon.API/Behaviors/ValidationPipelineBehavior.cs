using ErrorOr;
using FluentValidation;
using MediatR;

namespace Beacon.API.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, ErrorOr<TResponse>> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken cancellationToken)
    {
        var validationErrors = await GetValidationErrorsAsync(request, cancellationToken);
        return validationErrors.Any() ? validationErrors : await next();
    }

    private async Task<List<Error>> GetValidationErrorsAsync(TRequest request, CancellationToken ct)
    {
        var failures = new List<Error>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, ct);

            foreach (var error in result.Errors)
                failures.Add(Error.Validation(error.PropertyName, error.ErrorMessage));
        }

        return failures;
    }
}
