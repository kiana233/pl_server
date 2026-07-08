namespace PlServer.Network;

public sealed class ConnectionReceiveLoop
{
    private readonly PacketFrameReadBuffer _readBuffer;
    private readonly ReceivePipeline _receivePipeline;

    public ConnectionReceiveLoop(
        ReceivePipeline receivePipeline,
        PacketFrameReadBuffer? readBuffer = null)
    {
        _receivePipeline = receivePipeline ?? throw new ArgumentNullException(nameof(receivePipeline));
        _readBuffer = readBuffer ?? new PacketFrameReadBuffer();
    }

    public int BufferLength => _readBuffer.BufferLength;

    public async ValueTask<ConnectionReceiveResult> ProcessChunkAsync(
        ClientConnectionContext connection,
        byte[] chunk,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(chunk);
        cancellationToken.ThrowIfCancellationRequested();

        _readBuffer.Append(chunk);
        var splitResult = _readBuffer.TryReadFrames();
        var packets = new List<ReceivedPacketResult>();

        foreach (var frame in splitResult.Frames)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var packet = await _receivePipeline
                .ProcessFrameAsync(connection, frame, cancellationToken)
                .ConfigureAwait(false);
            packets.Add(packet);
        }

        return new ConnectionReceiveResult(
            packets,
            splitResult.Errors,
            splitResult.RemainingBufferLength,
            connection.CurrentSessionState);
    }

    public void Clear()
    {
        _readBuffer.Clear();
    }
}
