using CsvHelper;
using DataImporter.Entities;
using System.Globalization;
using DataImporter.Data;

namespace DataImporter;

public static class ContactCsvReader
{
    public static IEnumerable<ProjectContact> GetProjectContacts(Project[] projects)
    {
        using var reader = new StreamReader("Data\\contacts.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<ContactCsvModel>();
        
        foreach (var r in records)
        {
            if (ProjectCode.TryParse(r.ProjectCode, out var projectCode) &&
                projects.SingleOrDefault(p => p.ProjectCode == projectCode) is { } project)
            {
                yield return new ProjectContact
                {
                    Id = Guid.NewGuid(),
                    Name = r.ContactName,
                    EmailAddress = r.ContactEmail,
                    PhoneNumber = r.ContactPhone,
                    LaboratoryId = project.LaboratoryId,
                    ProjectId = project.Id
                };
            }
        }
    }
}
