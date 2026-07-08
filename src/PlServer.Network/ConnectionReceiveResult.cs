namespace PlServer.Network;

public sealed record ConnectionReceiveResult(
    IReadOnlyList<ReceivedPacketContext> Packets,
    IReadOnlyList<FrameSplitError> Errors,
    int RemainingBufferLength);
