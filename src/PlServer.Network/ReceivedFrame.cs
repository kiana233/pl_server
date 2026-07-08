namespace PlServer.Network;

public sealed record ReceivedFrame(
    byte[] FrameBytes,
    long OffsetInStream,
    DateTimeOffset ReceivedAtUtc,
    string SourceLabel,
    IReadOnlyList<string> Notes);
