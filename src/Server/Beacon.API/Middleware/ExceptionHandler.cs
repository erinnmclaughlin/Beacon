using Beacon.Common.Responses;
using FluentValidation;

namespace Beacon.API.Middleware;

public class ExceptionHandler : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            var response = new ValidationProblemResponse
            {
                Errors = e.Errors
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(f => f.ErrorMessage).Distinct().ToArray())
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 422;

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
