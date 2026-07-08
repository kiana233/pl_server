namespace PlServer.Network;

public sealed class PacketFrameReadBuffer
{
    private readonly List<byte> _buffer = new();
    private readonly PacketFrameSplitter _splitter;
    private long _bufferStartOffset;

    public PacketFrameReadBuffer(PacketFrameSplitter? splitter = null)
    {
        _splitter = splitter ?? new PacketFrameSplitter();
    }

    public int BufferLength => _buffer.Count;

    public void Append(byte[] chunk)
    {
        ArgumentNullException.ThrowIfNull(chunk);
        _buffer.AddRange(chunk);
    }

    public PacketFrameSplitResult TryReadFrames()
    {
        var result = _splitter.Split(_buffer, _bufferStartOffset);
        _bufferStartOffset += result.ConsumedBytes;
        return result;
    }

    public void Clear()
    {
        _bufferStartOffset += _buffer.Count;
        _buffer.Clear();
    }
}
