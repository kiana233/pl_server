using PlServer.Diagnostics;
using PlServer.Protocol;

namespace PlServer.Session;

public sealed record SessionPacketClassification(
    SessionPacketKind Kind,
    byte? Ac,
    byte? SubAc,
    ProtocolTraceSourceLabel SourceLabel,
    ProtocolTraceStatus Status,
    string Reason,
    IReadOnlyList<PacketValidationError>? ValidationErrors = null)
{
    public bool IsInvalid => Kind == SessionPacketKind.InvalidPacket;
}
