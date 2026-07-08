namespace PlServer.Network;

public sealed class ReceivePipeline
{
    private readonly PacketRoutePipeline _packetRoutePipeline;

    public ReceivePipeline(PacketRoutePipeline packetRoutePipeline)
    {
        _packetRoutePipeline = packetRoutePipeline ?? throw new ArgumentNullException(nameof(packetRoutePipeline));
    }

    public ValueTask<ReceivedPacketContext> ReceiveAsync(
        ClientConnectionContext connection,
        byte[] frameBytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(frameBytes);
        return _packetRoutePipeline.RouteAsync(connection, frameBytes, cancellationToken);
    }

    public ValueTask<ReceivedPacketContext> ProcessFrameAsync(
        ClientConnectionContext connection,
        ReceivedFrame frame,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(frame);
        return _packetRoutePipeline.RouteAsync(connection, frame.FrameBytes, cancellationToken);
    }
}
