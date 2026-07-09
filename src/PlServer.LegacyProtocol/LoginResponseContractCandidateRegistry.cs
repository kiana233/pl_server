namespace PlServer.LegacyProtocol;

public sealed class LoginResponseContractCandidateRegistry
{
    private readonly Dictionary<string, LoginResponseContractCandidate> candidatesByName = new(StringComparer.Ordinal);
    private readonly Dictionary<LoginResponseKind, LoginResponseContractCandidate> candidatesByKind = new();

    public static LoginResponseContractCandidateRegistry CreateDefault()
    {
        var registry = new LoginResponseContractCandidateRegistry();

        foreach (var candidate in CreateDefaultCandidates())
        {
            registry.Register(candidate);
        }

        return registry;
    }

    public static IReadOnlyList<LoginResponseContractCandidate> CreateDefaultCandidates()
    {
        return new[]
        {
            new LoginResponseContractCandidate(
                "LoginSuccessCandidate",
                LoginResponseKind.LoginSuccessCandidate,
                LegacyPacketDirection.S2C,
                ProtocolSourceLabel.Unknown,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                LoginResponseContractStatus.LayoutUnknown,
                requiredRequestContractName: "LoginRequestCandidate",
                requiredSessionState: LegacyProtocolSessionRequirement.LoginPending,
                resultSessionState: LegacyProtocolSessionRequirement.LoginAccepted,
                notes: new[]
                {
                    "layout unknown",
                    "requires sanitized target-client trace",
                    "candidate metadata only; no response bytes generated"
                }),
            new LoginResponseContractCandidate(
                "LoginFailureCandidate",
                LoginResponseKind.LoginFailureCandidate,
                LegacyPacketDirection.S2C,
                ProtocolSourceLabel.Unknown,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                LoginResponseContractStatus.LayoutUnknown,
                requiredRequestContractName: "LoginRequestCandidate",
                requiredSessionState: LegacyProtocolSessionRequirement.LoginPending,
                resultSessionState: LegacyProtocolSessionRequirement.Rejected,
                notes: new[]
                {
                    "layout unknown",
                    "requires sanitized target-client trace",
                    "candidate metadata only; no response bytes generated"
                }),
            new LoginResponseContractCandidate(
                "CharacterListFollowsCandidate",
                LoginResponseKind.CharacterListFollowsCandidate,
                LegacyPacketDirection.S2C,
                ProtocolSourceLabel.Unknown,
                ProtocolEvidenceStatus.PendingTargetClientTrace,
                LoginResponseContractStatus.LayoutUnknown,
                requiredRequestContractName: "LoginRequestCandidate",
                requiredSessionState: LegacyProtocolSessionRequirement.LoginAccepted,
                resultSessionState: LegacyProtocolSessionRequirement.CharacterListShown,
                notes: new[]
                {
                    "separate future task",
                    "no character list response generated",
                    "requires sanitized target-client trace"
                })
        };
    }

    public void Register(LoginResponseContractCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        if (candidatesByName.ContainsKey(candidate.Name))
        {
            throw new InvalidOperationException($"Duplicate login response candidate name: {candidate.Name}.");
        }

        candidatesByName.Add(candidate.Name, candidate);

        if (!candidatesByKind.ContainsKey(candidate.Kind))
        {
            candidatesByKind.Add(candidate.Kind, candidate);
        }
    }

    public IReadOnlyList<LoginResponseContractCandidate> GetAll()
    {
        return candidatesByName.Values.ToArray();
    }

    public bool TryResolveByName(
        string name,
        out LoginResponseContractCandidate? candidate)
    {
        candidate = null;

        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        return candidatesByName.TryGetValue(name, out candidate);
    }

    public bool TryResolveByKind(
        LoginResponseKind kind,
        out LoginResponseContractCandidate? candidate)
    {
        return candidatesByKind.TryGetValue(kind, out candidate);
    }
}
