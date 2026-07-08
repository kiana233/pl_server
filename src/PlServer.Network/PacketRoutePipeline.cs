using PlServer.Application;
using PlServer.Diagnostics;
using PlServer.LegacyProtocol;
using PlServer.Protocol;

namespace PlServer.Network;

public sealed class PacketRoutePipeline
{
    private readonly PacketCodec _packetCodec;
    private readonly ProtocolTraceLogger _traceLogger;
    private readonly ActionRouter _actionRouter;

    public PacketRoutePipeline(
        PacketCodec packetCodec,
        ProtocolTraceLogger traceLogger,
        ActionRouter actionRouter)
    {
        _packetCodec = packetCodec ?? throw new ArgumentNullException(nameof(packetCodec));
        _traceLogger = traceLogger ?? throw new ArgumentNullException(nameof(traceLogger));
        _actionRouter = actionRouter ?? throw new ArgumentNullException(nameof(actionRouter));
    }

    public async ValueTask<ReceivedPacketContext> RouteAsync(
        ClientConnectionContext connection,
        byte[] rawBytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(rawBytes);
        cancellationToken.ThrowIfCancellationRequested();

        var decodeResult = _packetCodec.Decode(rawBytes);

        var routeRequest = new ActionRouteRequest(
            connection.ConnectionId,
            connection.CurrentSessionState,
            decodeResult,
            LegacyPacketDirection.C2S,
            connection.AccountName,
            connection.CharacterName);

        var routeResult = await _actionRouter
            .RouteAsync(routeRequest, cancellationToken)
            .ConfigureAwait(false);

        var traceEvent = _traceLogger.CreatePacketDecodeEvent(
            ProtocolTraceDirection.C2S,
            connection.ConnectionId,
            decodeResult,
            connection.AccountName,
            connection.CharacterName,
            connection.CurrentSessionState.ToString(),
            result: "received",
            routeStatus: routeResult.Status.ToString(),
            handler: routeResult.HandlerName,
            handlerStatus: routeResult.HandlerStatus?.ToString(),
            handlerNotes: routeResult.HandlerNotes);

        return new ReceivedPacketContext(connection, rawBytes.ToArray(), decodeResult, routeResult, traceEvent);
    }

    public void WriteTrace(ProtocolTraceEvent traceEvent)
    {
        ArgumentNullException.ThrowIfNull(traceEvent);
        _traceLogger.Log(traceEvent);
        _traceLogger.Flush();
    }
}
