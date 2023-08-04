using CsvHelper.Configuration.Attributes;

namespace DataImporter.Data;

public sealed class ContactCsvModel
{
    [Name("Project ID")] 
    public string ProjectCode { get; set; } = "";

    [Name("City")]
    public string? City { get; set; }

    [Name("State")]
    public string? State { get; set; }

    [Name("Country")]
    public string? Country { get; set; }

    [Name("Lead")]
    public string? Lead { get; set; }

    [Name("Reviewer")]
    public string? Reviewer { get; set; }

    [Name("Sales")]
    public string? Sales { get; set; }

    [Name("Contact Name")]
    public string ContactName { get; set; } = "";

    [Name("Contact Email")]
    public string? ContactEmail { get; set; }

    [Name("Contact Phone")]
    public string? ContactPhone { get; set; }
}
