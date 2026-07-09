namespace PlServer.LegacyProtocol;

public sealed class CharacterListContractCandidateRegistry
{
    private readonly Dictionary<string, CharacterListContractCandidate> candidatesByName = new(StringComparer.Ordinal);
    private readonly Dictionary<CharacterListResponseKind, CharacterListContractCandidate> candidatesByKind = new();

    public static CharacterListContractCandidateRegistry CreateDefault()
    {
        var registry = new CharacterListContractCandidateRegistry();

        foreach (var candidate in CreateDefaultCandidates())
        {
            registry.Register(candidate);
        }

        return registry;
    }

    public static IReadOnlyList<CharacterListContractCandidate> CreateDefaultCandidates()
    {
        return new[]
        {
            new CharacterListContractCandidate(
                "CharacterListFollowsLoginCandidate",
                CharacterListResponseKind.CharacterListFollowsLoginCandidate,
                LegacyPacketDirection.S2C,
                ProtocolSourceLabel.Assumption,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                CharacterListContractStatus.LayoutUnknown,
                requiredRequestContractName: "LoginRequestCandidate",
                requiredSessionState: LegacyProtocolSessionRequirement.LoginAccepted,
                resultSessionState: LegacyProtocolSessionRequirement.CharacterListShown,
                notes: new[]
                {
                    "layout unknown",
                    "requires sanitized target-client trace",
                    "candidate metadata only; no character list bytes generated"
                }),
            new CharacterListContractCandidate(
                "EmptyCharacterListCandidate",
                CharacterListResponseKind.EmptyCharacterListCandidate,
                LegacyPacketDirection.S2C,
                ProtocolSourceLabel.Assumption,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                CharacterListContractStatus.LayoutUnknown,
                requiredRequestContractName: "LoginRequestCandidate",
                requiredSessionState: LegacyProtocolSessionRequirement.LoginAccepted,
                resultSessionState: LegacyProtocolSessionRequirement.CharacterListShown,
                notes: new[]
                {
                    "layout unknown",
                    "no bytes generated",
                    "requires sanitized target-client trace"
                }),
            new CharacterListContractCandidate(
                "CharacterListUnavailableCandidate",
                CharacterListResponseKind.CharacterListUnavailableCandidate,
                LegacyPacketDirection.S2C,
                ProtocolSourceLabel.Assumption,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                CharacterListContractStatus.LayoutUnknown,
                requiredRequestContractName: "LoginRequestCandidate",
                requiredSessionState: LegacyProtocolSessionRequirement.LoginAccepted,
                resultSessionState: LegacyProtocolSessionRequirement.LoginAccepted,
                notes: new[]
                {
                    "layout unknown",
                    "no bytes generated",
                    "requires sanitized target-client trace"
                })
        };
    }

    public void Register(CharacterListContractCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        if (candidatesByName.ContainsKey(candidate.Name))
        {
            throw new InvalidOperationException($"Duplicate character list candidate name: {candidate.Name}.");
        }

        candidatesByName.Add(candidate.Name, candidate);

        if (!candidatesByKind.ContainsKey(candidate.Kind))
        {
            candidatesByKind.Add(candidate.Kind, candidate);
        }
    }

    public IReadOnlyList<CharacterListContractCandidate> GetAll()
    {
        return candidatesByName.Values.ToArray();
    }

    public bool TryResolveByName(
        string name,
        out CharacterListContractCandidate? candidate)
    {
        candidate = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return candidatesByName.TryGetValue(name, out candidate);
    }

    public bool TryResolveByKind(
        CharacterListResponseKind kind,
        out CharacterListContractCandidate? candidate)
    {
        return candidatesByKind.TryGetValue(kind, out candidate);
    }
}
