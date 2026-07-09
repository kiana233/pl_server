namespace PlServer.Core;

public readonly record struct AccountName
{
    public AccountName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.Trim();
        NormalizedValue = Value.ToUpperInvariant();
    }

    public string Value { get; }

    public string NormalizedValue { get; }

    public override string ToString()
    {
        return Value;
    }
}
