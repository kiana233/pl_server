namespace PlServer.Replay;

public sealed class ReplayStep
{
    public int StepIndex { get; init; }

    public ReplayDirection Direction { get; init; } = ReplayDirection.Internal;

    public string RawHex { get; init; } = string.Empty;

    public string DecodedHex { get; init; } = string.Empty;

    public byte[] RawBytes { get; init; } = Array.Empty<byte>();

    public byte[] DecodedBytes { get; init; } = Array.Empty<byte>();

    public byte? ExpectedAc { get; init; }

    public byte? ExpectedSubAc { get; init; }

    public string? ProtocolName { get; init; }

    public string? Behavior { get; init; }

    public string? SourceLabel { get; init; }

    public string? Status { get; init; }
}
