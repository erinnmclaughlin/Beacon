namespace DataImporter;

public sealed record ProjectCode
{
    public string CustomerCode { get; init; }
    public string Date { get; init; }
    public int Suffix { get; init; }

    public ProjectCode(string customerCode, string date, int suffix)
    {
        if (customerCode.Length != 3)
            throw new ArgumentOutOfRangeException(nameof(customerCode));

        if (date.Length != 6)
            throw new ArgumentOutOfRangeException(nameof(date));

        if (suffix > 999)
            throw new ArgumentOutOfRangeException(nameof(suffix));

        CustomerCode = customerCode;
        Date = date;
        Suffix = suffix;
    }

    public static bool TryParse(string value, out ProjectCode projectCode)
    {
        var parts = value.Split('-');

        if (parts.Length != 3 || parts[0].Length != 3 || parts[1].Length != 6 || !int.TryParse(parts[2], out var suffix))
        {
            projectCode = default!;
            return false;
        }

        projectCode = new ProjectCode(parts[0], parts[1], suffix);
        return true;
    }

    public override string ToString()
    {
        return $"{CustomerCode}-{Suffix:000}";
    }
}
