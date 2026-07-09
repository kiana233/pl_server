namespace PlServer.LegacyProtocol;

public sealed class CharacterListContractCandidate
{
    public CharacterListContractCandidate(
        string name,
        CharacterListResponseKind kind,
        LegacyPacketDirection direction,
        ProtocolSourceLabel sourceLabel,
        ProtocolEvidenceStatus evidenceStatus,
        CharacterListContractStatus status,
        string? requiredRequestContractName = null,
        LegacyProtocolSessionRequirement? requiredSessionState = null,
        LegacyProtocolSessionRequirement? resultSessionState = null,
        IEnumerable<CharacterListFieldDescriptor>? fieldDescriptors = null,
        IEnumerable<string>? notes = null)
    {
        Name = RequireText(name, nameof(name));
        Kind = kind;
        Direction = direction;
        SourceLabel = sourceLabel;
        EvidenceStatus = evidenceStatus;
        Status = status;
        RequiredRequestContractName = requiredRequestContractName;
        RequiredSessionState = requiredSessionState;
        ResultSessionState = resultSessionState;
        FieldDescriptors = fieldDescriptors?.ToArray() ?? Array.Empty<CharacterListFieldDescriptor>();
        Notes = notes?.Where(note => !string.IsNullOrWhiteSpace(note)).ToArray() ?? Array.Empty<string>();
    }

    public string Name { get; }

    public CharacterListResponseKind Kind { get; }

    public LegacyPacketDirection Direction { get; }

    public ProtocolSourceLabel SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public CharacterListContractStatus Status { get; }

    public bool IsConfirmed => false;

    public string? RequiredRequestContractName { get; }

    public LegacyProtocolSessionRequirement? RequiredSessionState { get; }

    public LegacyProtocolSessionRequirement? ResultSessionState { get; }

    public IReadOnlyList<CharacterListFieldDescriptor> FieldDescriptors { get; }

    public IReadOnlyList<string> Notes { get; }

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
