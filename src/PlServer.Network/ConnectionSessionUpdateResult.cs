using PlServer.Session;

namespace PlServer.Network;

public sealed record ConnectionSessionUpdateResult(
    SessionState PreviousState,
    SessionState CurrentState,
    SessionPacketKind PacketKind,
    ConnectionSessionUpdateStatus Status,
    bool WasStateChanged,
    string? RejectionReason,
    IReadOnlyList<SessionTransitionError> Errors,
    IReadOnlyList<string> Notes);
