using PlServer.LegacyProtocol;

namespace PlServer.Application;

public sealed class LoginResponseCandidatePlanResult
{
    private LoginResponseCandidatePlanResult(
        LoginResponseCandidatePlanStatus status,
        LoginResponseContractCandidate? candidate,
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

    public LoginResponseCandidatePlanStatus Status { get; }

    public LoginResponseContractCandidate? Candidate { get; }

    public bool ShouldGeneratePacket => false;

    public byte[] GeneratedBytes => Array.Empty<byte>();

    public ProtocolSourceLabel SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public bool IsConfirmed => false;

    public IReadOnlyList<string> Notes { get; }

    public static LoginResponseCandidatePlanResult CannotPlanUnknownRequestLayout(
        LoginRequestParseResult parserResult)
    {
        ArgumentNullException.ThrowIfNull(parserResult);

        return new LoginResponseCandidatePlanResult(
            LoginResponseCandidatePlanStatus.CannotPlanUnknownRequestLayout,
            null,
            ProtocolSourceLabel.Unknown,
            parserResult.EvidenceStatus,
            new[]
            {
                "response planning status: CannotPlanUnknownRequestLayout",
                "login request field layout unknown",
                "no response generated",
                "pending target-client trace",
                "not confirmed"
            });
    }

    public static LoginResponseCandidatePlanResult CannotPlanWithoutAuthentication()
    {
        return new LoginResponseCandidatePlanResult(
            LoginResponseCandidatePlanStatus.CannotPlanWithoutAuthentication,
            null,
            ProtocolSourceLabel.Unknown,
            ProtocolEvidenceStatus.Unknown,
            new[]
            {
                "response planning status: CannotPlanWithoutAuthentication",
                "no account authentication result is available",
                "no response generated",
                "not confirmed"
            });
    }

    public static LoginResponseCandidatePlanResult CandidateOnlyNoPacketGenerated(
        LoginResponseContractCandidate candidate)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        return new LoginResponseCandidatePlanResult(
            LoginResponseCandidatePlanStatus.CandidateOnlyNoPacketGenerated,
            candidate,
            candidate.SourceLabel,
            candidate.EvidenceStatus,
            candidate.Notes.Concat(new[]
            {
                "response planning status: CandidateOnlyNoPacketGenerated",
                "response contract layout unknown",
                "no response generated",
                "no character list generated",
                "pending target-client trace",
                "not confirmed"
            }).ToArray());
    }
}
