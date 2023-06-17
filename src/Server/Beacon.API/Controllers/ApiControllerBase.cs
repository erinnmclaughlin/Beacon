using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Beacon.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _sender;
    private ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected Task<T> GetAsync<T>(IRequest<T> query, CancellationToken ct)
    {
        return Sender.Send(query, ct);
    }

    protected async Task ExecuteAsync(IRequest command, CancellationToken ct)
    {
        await Sender.Send(command, ct);
    }
}
