using CsvHelper.Configuration.Attributes;

namespace DataImporter.Data;

public sealed class InstrumentCsvModel
{
    [Name("Type")]
    public string Type { get; set; } = "";

    [Name("SN")]
    public string SN { get; set; } = "";
}
