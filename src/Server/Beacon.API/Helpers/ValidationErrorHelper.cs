using Beacon.Common.Validation;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Helpers;

public static class ValidationErrorHelper
{
    public static IActionResult ToValidationProblemResult(this IEnumerable<Error> errors)
    {
        var errorDictionary = errors
            .Where(e => e.Type is ErrorType.Validation)
            .GroupBy(e => e.Code)
            .ToDictionary(g => g.Key, g => g.Select(v => v.Description).ToArray());

        return new UnprocessableEntityObjectResult(new BeaconValidationProblem
        {
            Errors = errorDictionary
        });
    }
}
