using PlServer.Protocol;

namespace PlServer.Network;

public sealed class PacketFrameSplitterOptions
{
    public PacketCodecOptions CodecOptions { get; init; } = PacketCodecOptions.Default;

    public int MaxFrameSize { get; init; } = 64 * 1024;

    public string SourceLabel { get; init; } = "synthetic";

    public void Validate()
    {
        ArgumentNullException.ThrowIfNull(CodecOptions);
        CodecOptions.Validate();

        if (MaxFrameSize < CodecOptions.PayloadOffset + 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxFrameSize),
                MaxFrameSize,
                "MaxFrameSize must fit the configured header and at least one payload byte.");
        }
    }
}
