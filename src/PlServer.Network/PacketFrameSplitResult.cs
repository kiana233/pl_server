namespace PlServer.Network;

public sealed record PacketFrameSplitResult(
    IReadOnlyList<ReceivedFrame> Frames,
    IReadOnlyList<FrameSplitError> Errors,
    int RemainingBufferLength,
    int ConsumedBytes,
    PacketFrameSplitStatus Status);
