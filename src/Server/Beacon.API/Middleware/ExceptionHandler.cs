using Beacon.App.Exceptions;
using Beacon.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Middleware;

public static class ExceptionHandler
{
    public static async Task HandleException(HttpContext context)
    {
        var ex = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        if (ex is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

            var result = Results.UnprocessableEntity(new BeaconValidationProblem { Errors = errors });
            await result.ExecuteAsync(context);
        }

        if (ex is BeaconException beaconException)
        {
            var result = beaconException.Type switch
            {
                BeaconExceptionType.NotAuthorized => Results.Unauthorized(),
                BeaconExceptionType.NotFound => Results.NotFound(beaconException.Message),
                _ => Results.StatusCode(500)
            };

            await result.ExecuteAsync(context);
        }
    }
}
