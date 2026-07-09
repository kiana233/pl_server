using PlServer.LegacyProtocol;

namespace PlServer.Application;

public sealed class CharacterListCandidatePlanResult
{
    private CharacterListCandidatePlanResult(
        CharacterListCandidatePlanStatus status,
        CharacterListContractCandidate? candidate,
        ProtocolSourceLabel sourceLabel,
        ProtocolEvidenceStatus evidenceStatus,
        IEnumerable<string>? notes = null)
    {
        Status = status;
        Candidate = candidate;
        SourceLabel = sourceLabel;
        EvidenceStatus = evidenceStatus;
        Notes = notes?.Where(note => !string.IsNullOrWhiteSpace(note)).ToArray() ?? Array.Empty<string>();
    }

    public CharacterListCandidatePlanStatus Status { get; }

    public CharacterListContractCandidate? Candidate { get; }

    public bool ShouldGeneratePacket => false;

    public byte[] GeneratedBytes => Array.Empty<byte>();

    public ProtocolSourceLabel SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public bool IsConfirmed => false;

    public IReadOnlyList<string> Notes { get; }

    public static CharacterListCandidatePlanResult CannotPlanWithoutConfirmedLoginResponse(
        LoginResponseCandidatePlanResult? loginResponsePlan)
    {
        return new CharacterListCandidatePlanResult(
            CharacterListCandidatePlanStatus.CannotPlanWithoutConfirmedLoginResponse,
            null,
            loginResponsePlan?.SourceLabel ?? ProtocolSourceLabel.Unknown,
            loginResponsePlan?.EvidenceStatus ?? ProtocolEvidenceStatus.Unknown,
            new[]
            {
                "character list planning status: CannotPlanWithoutConfirmedLoginResponse",
                "login response packet is not confirmed",
                "login response packet was not generated",
                "character list layout unknown",
                "no character list generated",
                "no character list response bytes generated",
                "pending target-client trace",
                "not confirmed"
            });
    }

    public static CharacterListCandidatePlanResult CharacterRepositoryNotImplemented(
        LoginResponseCandidatePlanResult? loginResponsePlan)
    {
        return new CharacterListCandidatePlanResult(
            CharacterListCandidatePlanStatus.CharacterRepositoryNotImplemented,
            null,
            loginResponsePlan?.SourceLabel ?? ProtocolSourceLabel.Unknown,
            loginResponsePlan?.EvidenceStatus ?? ProtocolEvidenceStatus.PendingTargetClientTrace,
            new[]
            {
                "character list planning status: CharacterRepositoryNotImplemented",
                "character repository is not implemented",
                "no real character data queried",
                "character list layout unknown",
                "no character list generated",
                "no character list response bytes generated",
                "no character selection response generated",
                "no enter-map response generated",
                "pending target-client trace",
                "not confirmed"
            });
    }

    public static CharacterListCandidatePlanResult CandidateOnlyNoPacketGenerated(
        CharacterListContractCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return new CharacterListCandidatePlanResult(
            CharacterListCandidatePlanStatus.CandidateOnlyNoPacketGenerated,
            candidate,
            candidate.SourceLabel,
            candidate.EvidenceStatus,
            candidate.Notes.Concat(new[]
            {
                "character list planning status: CandidateOnlyNoPacketGenerated",
                "character list contract layout unknown",
                "no character list generated",
                "no character list response bytes generated",
                "no character selection response generated",
                "no enter-map response generated",
                "pending target-client trace",
                "not confirmed"
            }).ToArray());
    }
}
