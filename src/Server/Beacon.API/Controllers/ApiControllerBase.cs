using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();
}
