using Beacon.API.Persistence.Entities;
using CsvHelper;
using DataImporter.Data;
using System.Globalization;

namespace DataImporter;

public static class LaboratoryInstrumentCsvReader
{
    public static IEnumerable<LaboratoryInstrument> GetLaboratoryInstruments(Guid labId)
    {
        using var reader = new StreamReader("Data\\instruments.csv");
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<InstrumentCsvModel>();

        foreach (var r in records)
        {
            yield return new LaboratoryInstrument
            {
                Id = Guid.NewGuid(),
                InstrumentType = r.Type,
                SerialNumber = r.SN,
                LaboratoryId = labId
            };
        }
    }
}
