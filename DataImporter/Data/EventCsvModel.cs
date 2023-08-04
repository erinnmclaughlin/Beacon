using CsvHelper.Configuration.Attributes;

namespace DataImporter.Data;

public sealed class EventCsvModel
{
    [Name("Date")]
    public DateOnly Date { get; set; }

    [Name("Project ID")]
    public string ProjectCode { get; set; } = "";

    [Name("Who")]
    public string Who { get; set; } = "";

    [Name("What")]
    public string What { get; set; } = "";

    [Name("Equipment")]
    public string Equipment { get; set; } = "";

    [Name("Hours")]
    public double Hours { get; set; }
}
