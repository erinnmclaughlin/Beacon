namespace Beacon.Common.Models;

public sealed record ProjectCode
{
    public string CustomerCode { get; init; }
    public int Suffix { get; init; }

    public ProjectCode(string customerCode, int suffix)
    {
        if (customerCode.Length != 3)
            throw new ArgumentOutOfRangeException(nameof(customerCode));

        if (suffix > 999)
            throw new ArgumentOutOfRangeException(nameof(suffix));

        CustomerCode = customerCode;
        Suffix = suffix;
    }

    public static ProjectCode? FromString(string projectCode)
    {
        var parts = projectCode.Split('-');

        if (parts.Length != 2 || parts[0].Length != 3 || parts[1].Length != 3 || !int.TryParse(parts[1], out var suffix))
            return null;

        return new ProjectCode(parts[0], suffix);
    }

    public override string ToString()
    {
        return $"{CustomerCode}-{Suffix:000}";
    }
}
