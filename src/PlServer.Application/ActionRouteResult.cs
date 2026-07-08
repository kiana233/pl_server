using PlServer.LegacyProtocol;
using PlServer.Protocol;
using PlServer.Session;

namespace PlServer.Application;

public sealed class ActionRouteResult
{
    public ActionRouteResult(
        bool isRouted,
        ActionRouteStatus status,
        LegacyProtocolContract? contract,
        string? handlerName,
        SessionPacketKind? packetKind,
        bool sessionGuardAllowed,
        string? rejectionReason,
        IReadOnlyList<PacketValidationError>? validationErrors,
        string sourceLabel,
        ProtocolEvidenceStatus evidenceStatus,
        IReadOnlyList<string>? notes,
        ActionHandlerResult? handlerResult = null)
    {
        IsRouted = isRouted;
        Status = status;
        Contract = contract;
        HandlerName = handlerName;
        PacketKind = packetKind;
        SessionGuardAllowed = sessionGuardAllowed;
        RejectionReason = rejectionReason;
        ValidationErrors = validationErrors ?? Array.Empty<PacketValidationError>();
        SourceLabel = sourceLabel;
        EvidenceStatus = evidenceStatus;
        Notes = notes ?? Array.Empty<string>();
        HandlerResult = handlerResult;
    }

    public bool IsRouted { get; }

    public ActionRouteStatus Status { get; }

    public LegacyProtocolContract? Contract { get; }

    public string? HandlerName { get; }

    public SessionPacketKind? PacketKind { get; }

    public bool SessionGuardAllowed { get; }

    public string? RejectionReason { get; }

    public IReadOnlyList<PacketValidationError> ValidationErrors { get; }

    public string SourceLabel { get; }

    public ProtocolEvidenceStatus EvidenceStatus { get; }

    public IReadOnlyList<string> Notes { get; }

    public ActionHandlerResult? HandlerResult { get; }

    public ActionHandlerStatus? HandlerStatus => HandlerResult?.Status;

    public IReadOnlyList<string> HandlerNotes => HandlerResult?.Notes ?? Array.Empty<string>();

    public string? ProtocolName => Contract?.ProtocolName;

    public string? ChineseBehavior => Contract?.ChineseBehavior;

    public static ActionRouteResult FromHandler(
        LegacyProtocolContract contract,
        SessionPacketKind packetKind,
        SessionStateGuardResult sessionGuardResult,
        IReadOnlyList<PacketValidationError>? validationErrors,
        ActionHandlerResult handlerResult)
    {
        ArgumentNullException.ThrowIfNull(contract);
        ArgumentNullException.ThrowIfNull(sessionGuardResult);
        ArgumentNullException.ThrowIfNull(handlerResult);

        var notes = contract.Notes
            .Concat(handlerResult.Notes)
            .Concat(new[] { "candidate handler result only; no gameplay business executed" })
            .ToArray();

        return new ActionRouteResult(
            true,
            ActionRouteStatus.CandidateHandled,
            contract,
            handlerResult.HandlerName,
            packetKind,
            sessionGuardResult.Allowed,
            null,
            validationErrors,
            contract.SourceLabelText,
            contract.EvidenceStatus,
            notes,
            handlerResult);
    }
}
