namespace PlServer.Application;

public sealed class RedactedFieldValue
{
    public static readonly RedactedFieldValue NotCaptured = new("[not-captured]", true);

    public static readonly RedactedFieldValue Redacted = new("[redacted]", true);

    private RedactedFieldValue(string displayValue, bool isRedacted)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayValue);
        DisplayValue = displayValue;
        IsRedacted = isRedacted;
    }

    public string DisplayValue { get; }

    public bool IsRedacted { get; }

    public static RedactedFieldValue Public(string displayValue)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayValue);
        return new RedactedFieldValue(displayValue, false);
    }

    public static RedactedFieldValue FromSensitivity(
        LoginRequestFieldSensitivity sensitivity,
        string? displayValue = null)
    {
        return RequiresRedaction(sensitivity)
            ? Redacted
            : Public(displayValue ?? "[not-sensitive]");
    }

    public override string ToString()
    {
        return DisplayValue;
    }

    private static bool RequiresRedaction(LoginRequestFieldSensitivity sensitivity)
    {
        return sensitivity is LoginRequestFieldSensitivity.SecretCandidate
            or LoginRequestFieldSensitivity.TokenCandidate
            or LoginRequestFieldSensitivity.UnknownSensitive
            or LoginRequestFieldSensitivity.Redacted;
    }
}
