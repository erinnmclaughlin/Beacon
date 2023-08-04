using CsvHelper;
using DataImporter.Data;
using DataImporter.Entities;
using System.Globalization;

namespace DataImporter;

public static class ProjectCsvReader
{
    public static IEnumerable<Project> GetProjects(Guid labId, User[] users)
    {
        using var reader = new StreamReader("Data\\projects.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var erin = users.Single(u => u.DisplayName == "Erin");
        var records = csv.GetRecords<ProjectCsvModel>().ToList();
        
        foreach (var r in records)
        {
            if (ProjectCode.TryParse(r.ProjectId, out var projectCode))
            {
                yield return new Project
                {
                    Id = Guid.NewGuid(),
                    LeadAnalystId = users.FirstOrDefault(u => u.DisplayName == r.Lead)?.Id,
                    CreatedById = erin.Id,
                    CustomerName = r.CustomerName,
                    ProjectCode = projectCode,
                    ProjectStatus = GetStatus(r.Status),
                    LaboratoryId = labId
                };
            }
        }
    }

    private static string GetStatus(string initialStatus) => initialStatus switch
    {
        "Cancelled" => "Canceled",
        "EU Lab" => "Expired",
        "On Hold" => "OnHold",
        "Current" => "Active",
        _ => initialStatus
    };
}
