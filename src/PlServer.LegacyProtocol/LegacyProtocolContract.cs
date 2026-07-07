namespace PlServer.LegacyProtocol;

public sealed class LegacyProtocolContract
{
    public LegacyProtocolContract(
        LegacyProtocolKey key,
        string protocolName,
        string chineseBehavior,
        string? englishBehavior,
        ProtocolSourceLabel sourceLabel,
        ProtocolEvidenceStatus evidenceStatus,
        LegacyProtocolSessionRequirement? requiredSessionState = null,
        LegacyProtocolSessionRequirement? resultSessionState = null,
        IEnumerable<LegacyPacketDirection>? allowedDirections = null,
        IEnumerable<LegacyProtocolFieldDescriptor>? fieldDescriptors = null,
        IEnumerable<string>? notes = null)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
        ProtocolName = RequireText(protocolName, nameof(protocolName));
        ChineseBehavior = RequireText(chineseBehavior, nameof(chineseBehavior));
        EnglishBehavior = englishBehavior;
        SourceLabel = sourceLabel;
        EvidenceStatus = evidenceStatus;
        RequiredSessionState = requiredSessionState;
        ResultSessionState = resultSessionState;
        AllowedDirections = MaterializeDirections(allowedDirections);
        FieldDescriptors = fieldDescriptors?.ToArray() ?? Array.Empty<LegacyProtocolFieldDescriptor>();
        Notes = notes?.Where(note => !string.IsNullOrWhiteSpace(note)).ToArray() ?? Array.Empty<string>();
    }

    public LegacyProtocolKey Key { get; }

    public string ProtocolName { get; }

    public string ChineseBehavior { get; }

    public string? EnglishBehavior { get; }

    public ProtocolSourceLabel SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public LegacyProtocolSessionRequirement? RequiredSessionState { get; }

    public LegacyProtocolSessionRequirement? ResultSessionState { get; }

    public IReadOnlyList<LegacyPacketDirection> AllowedDirections { get; }

    public IReadOnlyList<LegacyProtocolFieldDescriptor> FieldDescriptors { get; }

    public IReadOnlyList<string> Notes { get; }

    public string SourceLabelText => SourceLabel.ToContractString();

    private static IReadOnlyList<LegacyPacketDirection> MaterializeDirections(
        IEnumerable<LegacyPacketDirection>? allowedDirections)
    {
        var directions = allowedDirections?.Distinct().ToArray() ?? Array.Empty<LegacyPacketDirection>();
        return directions.Length == 0
            ? new[] { LegacyPacketDirection.Any }
            : directions;
    }

    private static string RequireText(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be empty.", parameterName);
        }

        return value;
    }
}
