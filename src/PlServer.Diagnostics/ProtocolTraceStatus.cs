namespace PlServer.Diagnostics;

public enum ProtocolTraceStatus
{
    Confirmed,
    PendingTargetClientTrace,
    Assumption,
    Unknown,
    Invalid,
    Rejected
}
