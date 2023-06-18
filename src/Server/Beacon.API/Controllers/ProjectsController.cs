using Beacon.App.Features.Projects;
using Beacon.App.Services;
using Beacon.App.ValueObjects;
using Beacon.Common.Projects.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beacon.API.Controllers;

[Authorize(AuthConstants.LabAuth), Route("api/projects")]
public class ProjectsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjects(CancellationToken ct)
    {
        return Ok(await GetAsync(new GetProjects.Query(), ct));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectRequest request, IProjectCodeGenerator codeGenerator, CancellationToken ct)
    {
        var projectCode = await codeGenerator.Generate(request.CustomerCode, ct);
        var command = new CreateProject.Command(projectCode, request.CustomerName);
        await ExecuteAsync(command, ct);

        var project = await GetAsync(new GetProjectByCode.Query(projectCode), ct);
        return Created($"api/projects/{project?.ProjectCode}", project);
    }

    [HttpGet("{projectCode}")]
    public async Task<IActionResult> GetProject(string projectCode, CancellationToken ct)
    {
        if (ProjectCode.FromString(projectCode) is not { } code)
            return BadRequest($"{projectCode} was not in a recognized format.");

        var projectOrNull = await GetAsync(new GetProjectByCode.Query(code), ct);
        return projectOrNull is { } project ? Ok(project) : NotFound("Project not found.");
    }

    [HttpPost("{projectCode}/cancel")]
    public async Task<IActionResult> CancelProject(string projectCode, CancellationToken ct)
    {
        if (ProjectCode.FromString(projectCode) is not { } code)
            return BadRequest($"{projectCode} was not in a recognized format.");

        await ExecuteAsync(new CancelProject.Command(code), ct);
        return NoContent();
    }

    [HttpPost("{projectCode}/complete")]
    public async Task<IActionResult> CompleteProject(string projectCode, CancellationToken ct)
    {
        if (ProjectCode.FromString(projectCode) is not { } code)
            return BadRequest($"{projectCode} was not in a recognized format.");

        await ExecuteAsync(new CompleteProject.Command(code), ct);
        return NoContent();
    }
}
