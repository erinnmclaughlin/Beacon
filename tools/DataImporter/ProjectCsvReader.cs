﻿using Beacon.API.Persistence.Entities;
using Beacon.Common.Models;
using CsvHelper;
using DataImporter.Data;
using System.Globalization;

namespace DataImporter;

public static class ProjectCsvReader
{
    public static IEnumerable<string> GetApplications()
    {
        using var reader = new StreamReader("Data\\projects.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<ProjectCsvModel>();

        foreach (var r in records)
            if (!string.IsNullOrWhiteSpace(r.Application))
                yield return r.Application;
    }

    public static IEnumerable<Project> GetProjects(ProjectApplication[] applications, User[] users)
    {
        using var reader = new StreamReader("Data\\projects.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var erin = users.Single(u => u.DisplayName == "Erin");
        var records = csv.GetRecords<ProjectCsvModel>().ToList();
        
        foreach (var r in records)
        {
            if (ProjectCode.TryParse(r.ProjectId, out var projectCode))
            {
                var project = new Project
                {
                    Id = Guid.NewGuid(),
                    LeadAnalystId = users.FirstOrDefault(u => u.DisplayName == r.Lead)?.Id,
                    CreatedOn = r.ClearedDate?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc) ?? DateTimeOffset.MinValue,
                    CreatedById = erin.Id,
                    CustomerName = r.CustomerName,
                    ProjectCode = projectCode,
                    ProjectStatus = GetStatus(r.Status)
                };

                if (!string.IsNullOrWhiteSpace(r.Application))
                {
                    project.TaggedApplications.Add(new ProjectApplicationTag
                    {
                        ApplicationId = applications.Single(a => a.Name == r.Application).Id
                    });
                }

                if (r.ProductName?.ToLower().Trim() is { } product && !string.IsNullOrWhiteSpace(product) && product != "na" && product != "n/a" && product != "none" && product != "empty")
                {
                    project.SampleGroups.AddRange(product.Split(",").Select(productName => new SampleGroup
                    {
                        Id = Guid.NewGuid(),
                        IsHazardous = r.ProductSeverity >= 3,
                        SampleName = productName.Trim(),
                        Notes = r.ProductType
                    }));
                }

                yield return project;
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
