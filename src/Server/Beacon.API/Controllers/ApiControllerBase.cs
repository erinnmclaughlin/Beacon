using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IActionResult ValidationProblem(IEnumerable<Error> errors)
    {
        var errorDictionary = errors
            .Where(e => e.Type is ErrorType.Validation)
            .GroupBy(e => e.Code)
            .ToDictionary(g => g.Key, g => g.Select(v => v.Description).ToArray());

        return new UnprocessableEntityObjectResult(new ValidationProblemDetails(errorDictionary));
    }
}
