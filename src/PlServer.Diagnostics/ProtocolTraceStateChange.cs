namespace PlServer.Diagnostics;

public sealed record ProtocolTraceStateChange(
    string PreviousState,
    string CurrentState,
    string PacketKind,
    bool WasStateChanged,
    string? RejectionReason,
    IReadOnlyList<ProtocolTraceStateChangeError> Errors,
    IReadOnlyList<string> Notes);

public sealed record ProtocolTraceStateChangeError(
    string Code,
    string Message);
