using PlServer.Diagnostics;

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
        var stateChange = ToProtocolTraceStateChange(updateResult);
        var traceEvent = packet.TraceEvent.WithStateChange(
            stateChange,
            updateResult.CurrentState.ToString());
        _packetRoutePipeline.WriteTrace(traceEvent);

        return new ReceivedPacketResult(
            packet.Connection,
            packet.RawBytes,
            packet.DecodeResult,
            packet.RouteResult,
            updateResult);
    }

    private static ProtocolTraceStateChange ToProtocolTraceStateChange(
        ConnectionSessionUpdateResult updateResult)
    {
        return new ProtocolTraceStateChange(
            updateResult.PreviousState.ToString(),
            updateResult.CurrentState.ToString(),
            updateResult.PacketKind.ToString(),
            updateResult.WasStateChanged,
            updateResult.RejectionReason,
            updateResult.Errors
                .Select(error => new ProtocolTraceStateChangeError(
                    error.Code.ToString(),
                    error.Message))
                .ToArray(),
            updateResult.Notes.ToArray());
    }
}
