namespace PlServer.Network;

public sealed record FrameSplitError(
    FrameSplitErrorCode Code,
    string Message,
    long OffsetInStream,
    int BytesDiscarded);
