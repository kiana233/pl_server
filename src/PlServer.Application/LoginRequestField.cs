using PlServer.LegacyProtocol;

namespace PlServer.Application;

public sealed class LoginRequestField
{
    public LoginRequestField(
        string fieldName,
        LoginRequestFieldKind fieldKind,
        int? offset,
        int? length,
        LoginRequestFieldSensitivity sensitivity,
        RedactedFieldValue redactedValue,
        string sourceLabel,
        ProtocolEvidenceStatus evidenceStatus,
        IReadOnlyList<string>? notes = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        ArgumentNullException.ThrowIfNull(redactedValue);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceLabel);

        FieldName = fieldName;
        FieldKind = fieldKind;
        Offset = offset;
        Length = length;
        Sensitivity = sensitivity;
        RedactedValue = redactedValue;
        SourceLabel = sourceLabel;
        EvidenceStatus = evidenceStatus;
        Notes = notes ?? Array.Empty<string>();
    }

    public string FieldName { get; }

    public LoginRequestFieldKind FieldKind { get; }

    public int? Offset { get; }

    public int? Length { get; }

    public LoginRequestFieldSensitivity Sensitivity { get; }

    public RedactedFieldValue RedactedValue { get; }

    public string SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public IReadOnlyList<string> Notes { get; }
}
