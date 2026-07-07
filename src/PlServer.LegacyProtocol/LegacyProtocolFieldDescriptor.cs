namespace PlServer.LegacyProtocol;

public sealed class LegacyProtocolFieldDescriptor
{
    public LegacyProtocolFieldDescriptor(
        string fieldName,
        int? offset,
        int? length,
        string? dataType,
        string description,
        ProtocolSourceLabel sourceLabel,
        ProtocolEvidenceStatus evidenceStatus)
    {
        FieldName = RequireText(fieldName, nameof(fieldName));
        Offset = offset;
        Length = length;
        DataType = dataType;
        Description = RequireText(description, nameof(description));
        SourceLabel = sourceLabel;
        EvidenceStatus = evidenceStatus;
    }

    public string FieldName { get; }

    public int? Offset { get; }

    public int? Length { get; }

    public string? DataType { get; }

    public string Description { get; }

    public ProtocolSourceLabel SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public string SourceLabelText => SourceLabel.ToContractString();

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty.", parameterName);
        }

        return value;
    }
}
