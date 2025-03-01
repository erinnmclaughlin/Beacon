using Beacon.Common.Validation;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Extensions;

public static class ResultToHttpResultMapper
{
    public static IResult ToHttpResult<T>(this ErrorOr<T> errorOrValue)
    {
        if (!errorOrValue.IsError) 
            return GetSuccessResult(errorOrValue.Value);
        
        if (errorOrValue.Errors.Any(e => e.NumericType == 401)) 
            return Results.Unauthorized();
        
        if (errorOrValue.Errors.Any(e => e.NumericType == 403))
            return Results.Forbid();

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
    
    private static IResult GetSuccessResult<T>(T? value) => value is null or Success ? Results.NoContent() : Results.Ok(value);
}
