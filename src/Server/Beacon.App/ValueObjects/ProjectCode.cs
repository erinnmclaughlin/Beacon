namespace Beacon.App.ValueObjects;

public sealed record ProjectCode
{
    public required string CompanyCode { get; init; }
    public required int Suffix { get; init; }

    public ProjectCode(string companyCode, int suffix)
    {
        if (companyCode.Length != 3)
            throw new ArgumentOutOfRangeException(nameof(companyCode));

        if (suffix > 999)
            throw new ArgumentOutOfRangeException(nameof(suffix));

        CompanyCode = companyCode;
        Suffix = suffix;
    }

    public override string ToString()
    {
        return $"{CompanyCode}-{Suffix:000}";
    }
}
