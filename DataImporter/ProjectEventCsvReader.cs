using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using CsvHelper;
using DataImporter.Data;
using System.Globalization;

namespace DataImporter;

public static class ProjectEventCsvReader
{
    public static IEnumerable<ProjectEvent> GetProjectEvents(Project[] projects, LaboratoryInstrument[] instruments)
    {
        using var reader = new StreamReader("Data\\events.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<EventCsvModel>();

        foreach (var r in records)
        {
            if (ProjectCode.TryParse(r.ProjectCode, out var projectCode) &&
                projects.SingleOrDefault(p => p.ProjectCode == projectCode) is { } project)
            {
                yield return new ProjectEvent
                {
                    Id = Guid.NewGuid(),
                    Title = r.What,
                    ScheduledStart = r.Date.ToDateTime(TimeOnly.MinValue),
                    ScheduledEnd = r.Date.ToDateTime(TimeOnly.MinValue).AddHours(r.Hours ?? 0),
                    LaboratoryId = project.LaboratoryId,
                    ProjectId = project.Id,
                    AssociatedInstruments = instruments.Where(i => r.Equipment == i.SerialNumber).ToList()
                };
            }
        }
    }
}
