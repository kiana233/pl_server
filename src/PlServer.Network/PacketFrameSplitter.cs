using PlServer.Protocol;

namespace PlServer.Network;

public sealed class PacketFrameSplitter
{
    private readonly PacketCodecOptions _codecOptions;
    private readonly int _maxFrameSize;
    private readonly string _sourceLabel;

    public PacketFrameSplitter(PacketFrameSplitterOptions? options = null)
    {
        var effectiveOptions = options ?? new PacketFrameSplitterOptions();
        effectiveOptions.Validate();
        _codecOptions = effectiveOptions.CodecOptions;
        _maxFrameSize = effectiveOptions.MaxFrameSize;
        _sourceLabel = effectiveOptions.SourceLabel;
    }

    public PacketFrameSplitResult Split(IList<byte> buffer, long bufferStartOffset)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        var frames = new List<ReceivedFrame>();
        var errors = new List<FrameSplitError>();
        var consumedBytes = 0;

        while (buffer.Count > 0)
        {
            var headerIndex = FindHeader(buffer);

            if (headerIndex < 0)
            {
                consumedBytes += DiscardNoiseWithoutHeader(buffer, bufferStartOffset + consumedBytes, errors);
                break;
            }

            if (headerIndex > 0)
            {
                consumedBytes += Discard(buffer, headerIndex);
                errors.Add(new FrameSplitError(
                    FrameSplitErrorCode.LeadingNoiseDiscarded,
                    "Discarded leading bytes before the next configured packet header.",
                    bufferStartOffset + consumedBytes - headerIndex,
                    headerIndex));
            }

            if (buffer.Count < _codecOptions.HeaderLength)
            {
                break;
            }

            var payloadLength = ReadPayloadLength(buffer);
            var frameLength = _codecOptions.PayloadOffset + payloadLength;

            if (payloadLength == 0)
            {
                errors.Add(new FrameSplitError(
                    FrameSplitErrorCode.InvalidDeclaredLength,
                    "Declared payload length is zero; frame splitter emitted the malformed frame for PacketCodec validation.",
                    bufferStartOffset + consumedBytes,
                    frameLength));

                var malformedFrameBytes = buffer.Take(frameLength).ToArray();
                frames.Add(new ReceivedFrame(
                    malformedFrameBytes,
                    bufferStartOffset + consumedBytes,
                    DateTimeOffset.UtcNow,
                    _sourceLabel,
                    new[] { "synthetic malformed stream frame; not target-client trace" }));
                consumedBytes += Discard(buffer, frameLength);
                continue;
            }

            if (frameLength > _maxFrameSize)
            {
                errors.Add(new FrameSplitError(
                    FrameSplitErrorCode.FrameTooLarge,
                    $"Declared frame length {frameLength} exceeds MaxFrameSize {_maxFrameSize}.",
                    bufferStartOffset + consumedBytes,
                    1));
                consumedBytes += Discard(buffer, 1);
                continue;
            }

            if (buffer.Count < frameLength)
            {
                break;
            }

            var frameBytes = buffer.Take(frameLength).ToArray();
            frames.Add(new ReceivedFrame(
                frameBytes,
                bufferStartOffset + consumedBytes,
                DateTimeOffset.UtcNow,
                _sourceLabel,
                new[] { "synthetic stream frame; not target-client trace" }));
            consumedBytes += Discard(buffer, frameLength);
        }

        return new PacketFrameSplitResult(
            frames,
            errors,
            buffer.Count,
            consumedBytes,
            GetStatus(frames.Count, errors.Count));
    }

    private int FindHeader(IList<byte> buffer)
    {
        for (var index = 0; index < buffer.Count - 1; index++)
        {
            if (buffer[index] == _codecOptions.HeaderByte0
                && buffer[index + 1] == _codecOptions.HeaderByte1)
            {
                return index;
            }
        }

        return -1;
    }

    private int DiscardNoiseWithoutHeader(
        IList<byte> buffer,
        long offsetInStream,
        ICollection<FrameSplitError> errors)
    {
        var keepTrailingHeaderCandidate = buffer[buffer.Count - 1] == _codecOptions.HeaderByte0;
        var bytesToDiscard = keepTrailingHeaderCandidate
            ? buffer.Count - 1
            : buffer.Count;

        if (bytesToDiscard > 0)
        {
            Discard(buffer, bytesToDiscard);
            errors.Add(new FrameSplitError(
                FrameSplitErrorCode.MalformedBytesDiscarded,
                "Discarded bytes that did not contain a complete configured packet header.",
                offsetInStream,
                bytesToDiscard));
        }

        return bytesToDiscard;
    }

    private ushort ReadPayloadLength(IList<byte> buffer)
    {
        return (ushort)(buffer[_codecOptions.LengthOffset] | (buffer[_codecOptions.LengthOffset + 1] << 8));
    }

    private static int Discard(IList<byte> buffer, int count)
    {
        for (var index = 0; index < count; index++)
        {
            buffer.RemoveAt(0);
        }

        return count;
    }

    private static PacketFrameSplitStatus GetStatus(int frameCount, int errorCount)
    {
        return (frameCount, errorCount) switch
        {
            (> 0, > 0) => PacketFrameSplitStatus.FramesAndErrors,
            (> 0, _) => PacketFrameSplitStatus.FramesAvailable,
            (_, > 0) => PacketFrameSplitStatus.Errors,
            _ => PacketFrameSplitStatus.NoFrame
        };
    }
}
