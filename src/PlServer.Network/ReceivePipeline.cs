namespace PlServer.Network;

public sealed class ReceivePipeline
{
    private readonly PacketRoutePipeline _packetRoutePipeline;
    private readonly ConnectionSessionUpdater _sessionUpdater;

    public ReceivePipeline(
        PacketRoutePipeline packetRoutePipeline,
        ConnectionSessionUpdater? sessionUpdater = null)
    {
        _packetRoutePipeline = packetRoutePipeline ?? throw new ArgumentNullException(nameof(packetRoutePipeline));
        _sessionUpdater = sessionUpdater ?? new ConnectionSessionUpdater();
    }

    public async ValueTask<ReceivedPacketResult> ReceiveAsync(
        ClientConnectionContext connection,
        byte[] frameBytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(frameBytes);
        var packet = await _packetRoutePipeline
            .RouteAsync(connection, frameBytes, cancellationToken)
            .ConfigureAwait(false);

        return ApplySessionUpdate(packet);
    }

    public async ValueTask<ReceivedPacketResult> ProcessFrameAsync(
        ClientConnectionContext connection,
        ReceivedFrame frame,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(frame);
        var packet = await _packetRoutePipeline
            .RouteAsync(connection, frame.FrameBytes, cancellationToken)
            .ConfigureAwait(false);

        return ApplySessionUpdate(packet);
    }

    private ReceivedPacketResult ApplySessionUpdate(ReceivedPacketContext packet)
    {
        var updateResult = _sessionUpdater.Update(
            packet.Connection,
            packet.DecodeResult,
            packet.RouteResult);

        return new ReceivedPacketResult(
            packet.Connection,
            packet.RawBytes,
            packet.DecodeResult,
            packet.RouteResult,
            updateResult);
    }
}
