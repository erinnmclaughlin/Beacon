using Beacon.Common.Validation;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Beacon.API;

public static class ResultToHttpResultMapper
{
    public static IResult ToHttpResult<T>(this ErrorOr<T> errorOrValue)
    {
        if (!errorOrValue.IsError)
        {
            var value = errorOrValue.Value;
            return value is null || value.GetType() == typeof(Success) ? Results.NoContent() : Results.Ok(value);
        }

        var errors = errorOrValue.Errors;

        if (errors.Any(e => e.NumericType == 401))
        {
            return Results.Unauthorized();
        }

        if (errors.Any(e => e.NumericType == 403))
        {
            return Results.Forbid();
        }

        if (errorOrValue.Errors.Where(e => e.Type == ErrorType.Validation).ToList() is { Count: > 0 } validationErrors)
        {
            return Results.UnprocessableEntity(new BeaconValidationProblem
            {
                Errors = validationErrors
                    .GroupBy(v => v.Code)
                    .ToDictionary(x => x.Key, x => x.Select(v => v.Description).ToArray())
            });
        }

        return Results.StatusCode(500);
    }
}
