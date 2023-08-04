﻿using CsvHelper.Configuration.Attributes;

namespace DataImporter;

public sealed class CsvData
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

    [Name("LHI")]
    public int? LHI { get; set; }

    [Name("Product Name")]
    public string? ProductName { get; set; }

    [Name("Type")]
    public string? ProductType { get; set; }

    [Name("Status")]
    public string Status { get; set; } = "";
}
