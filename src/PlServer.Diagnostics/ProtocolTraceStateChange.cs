namespace PlServer.Diagnostics;

public sealed record ProtocolTraceStateChange(
    string? FromState,
    string? ToState,
    string? Reason);
