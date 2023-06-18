using Beacon.App.Entities;
using Beacon.App.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Beacon.App.Services;

public interface IProjectCodeGenerator
{
    Task<ProjectCode> Generate(string customerCode, CancellationToken ct);
}

internal sealed class ProjectCodeGenerator : IProjectCodeGenerator
{
    private readonly ICurrentLab _currentLab;
    private readonly IQueryService _queryService;

    public ProjectCodeGenerator(ICurrentLab currentLab, IQueryService queryService)
    {
        _currentLab = currentLab;
        _queryService = queryService;
    }

    public async Task<ProjectCode> Generate(string customerCode, CancellationToken ct)
    {
        if (customerCode.Length != 3)
            throw new ArgumentException("Customer code must be exactly 3 characters.");

        var labId = _currentLab.LabId;

        var lastSuffix = await _queryService
            .QueryFor<Project>()
            .Where(p => p.LaboratoryId == labId && p.ProjectCode.CustomerCode == customerCode)
            .OrderBy(p => p.ProjectCode.Suffix)
            .Select(p => p.ProjectCode.Suffix)
            .LastOrDefaultAsync(ct);

        return new ProjectCode(customerCode, lastSuffix + 1);
    }
}
