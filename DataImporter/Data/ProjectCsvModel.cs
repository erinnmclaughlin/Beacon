using CsvHelper.Configuration.Attributes;

namespace DataImporter.Data;

public sealed class ProjectCsvModel
{
    [Name("Customer Name")]
    public string CustomerName { get; set; } = "";

    [Name("Project ID")]
    public string ProjectId { get; set; } = "";

    [Name("Cleared Date")]
    public DateOnly? ClearedDate { get; set; }

    [Name("Lead")]
    public string? Lead { get; set; }

    [Name("Project Type")]
    public string? ProjectType { get; set; }

    [Name("Application")]
    public string? Application { get; set; }

    [Name("Product Severity")]
    public int? ProductSeverity { get; set; }

    [Name("Product Name")]
    public string? ProductName { get; set; }

    [Name("Product Type")]
    public string? ProductType { get; set; }

    [Name("Status")]
    public string Status { get; set; } = "";
}
