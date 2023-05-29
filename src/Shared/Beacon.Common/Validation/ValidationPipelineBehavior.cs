using ErrorOr;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Beacon.Common.Validation;

public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, ErrorOr<TResponse>>
    where TRequest : IApiRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<ErrorOr<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ErrorOr<TResponse>> next, CancellationToken ct)
    {
        var validationErrors = await GetValidationErrorsAsync(request, ct);
        return validationErrors.Any() ? validationErrors : await next();
    }

    private async Task<List<Error>> GetValidationErrorsAsync(TRequest request, CancellationToken ct)
    {
        if (!_validators.Any())
            return new List<Error>();

        var failures = new List<ValidationFailure>();

        foreach (var validator in _validators)
        {
            var result = await validator.ValidateAsync(request, ct);

            if (result.Errors.Any())
                failures.AddRange(result.Errors);
        }

        return failures.Select(f => Error.Validation(f.PropertyName, f.ErrorMessage)).ToList();
    }
}
