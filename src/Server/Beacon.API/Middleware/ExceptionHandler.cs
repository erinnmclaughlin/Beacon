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
            await HandleValidationException(validationException, context);
            return;
        }
        
        if (ex is UserNotAllowedException)
        {
            await Results.Forbid().ExecuteAsync(context);
            return;
        }

        if (ex is InvalidOperationException)
        {
            await Results.BadRequest(ex.Message).ExecuteAsync(context);
            return;
        }

        await Results.StatusCode(500).ExecuteAsync(context);
    }

    private static async Task HandleValidationException(ValidationException ex, HttpContext context)
    {
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).Distinct().ToArray());

        var result = Results.UnprocessableEntity(new BeaconValidationProblem { Errors = errors });
        await result.ExecuteAsync(context);
    }
}
