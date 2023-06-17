using Beacon.App.Features.Projects;
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
    public async Task<IActionResult> CreateProject(CreateProjectRequest request, CancellationToken ct)
    {
        var command = new CreateProject.Command(Guid.NewGuid(), request.CustomerCode, request.CustomerName);
        await ExecuteAsync(command, ct);

        var project = await GetAsync(new GetProjectById.Query(command.Id), ct);
        return Created($"api/projects/{project?.Id}", project);
    }

    [HttpGet("{idOrProjectCode}")]
    public async Task<IActionResult> GetProjectByIdOrCode(string idOrProjectCode, CancellationToken ct)
    {
        if (Guid.TryParse(idOrProjectCode, out var id))
        {
            var projectOrNull = await GetAsync(new GetProjectById.Query(id), ct);
            return projectOrNull is { } project ? Ok(project) : NotFound("Project not found.");
        }

        if (ProjectCode.FromString(idOrProjectCode) is { } projectCode)
        {
            var projectOrNull = await GetAsync(new GetProjectByCode.Query(projectCode), ct);
            return projectOrNull is { } project ? Ok(project) : NotFound("Project not found.");
        }

        return BadRequest($"{idOrProjectCode} was not in a recognized format.");
    }
}
