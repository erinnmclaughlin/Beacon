using Beacon.Common.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Beacon.API.Middleware;

public class ApiExceptionHandler : IMiddleware
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
            context.Response.StatusCode = 422; // Unprocessable Content

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
