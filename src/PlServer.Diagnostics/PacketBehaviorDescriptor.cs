namespace PlServer.Diagnostics;

public sealed record PacketBehaviorDescriptor(
    string ProtocolName,
    string Behavior,
    ProtocolTraceSourceLabel SourceLabel,
    ProtocolTraceStatus Status,
    string? Handler = null);
