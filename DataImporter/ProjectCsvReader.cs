﻿using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using CsvHelper;
using DataImporter.Data;
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

    private static ProjectStatus GetStatus(string initialStatus) => initialStatus switch
    {
        "Cancelled" => ProjectStatus.Canceled,
        "EU Lab" => ProjectStatus.Expired,
        "On Hold" => ProjectStatus.OnHold,
        "Current" => ProjectStatus.Active,
        _ => Enum.Parse<ProjectStatus>(initialStatus)
    };
}
