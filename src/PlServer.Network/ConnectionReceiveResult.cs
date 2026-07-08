namespace PlServer.Network;

public sealed record ConnectionReceiveResult(
    IReadOnlyList<ReceivedPacketResult> Packets,
    IReadOnlyList<FrameSplitError> Errors,
    int RemainingBufferLength,
    PlServer.Session.SessionState FinalSessionState);
