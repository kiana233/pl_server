namespace PlServer.Diagnostics;

public sealed record ProtocolTraceResourceCheck(
    string Name,
    string Result,
    ProtocolTraceSourceLabel SourceLabel = ProtocolTraceSourceLabel.Unknown,
    ProtocolTraceStatus Status = ProtocolTraceStatus.Unknown);
